using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.Npgsql
{
    public class NpgsqlTableInspector : DbTableInspectorBase
    {
        #region Constructor

        public NpgsqlTableInspector(IDbConnection connection, string schema, string tableName)
            : base(
                connection,
                schema ?? NpgsqlInspector.DefaultSchema,
                tableName)
        {
        }

        #endregion

        #region Private

        private static bool ParseBoolean(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value is bool b)
            {
                return b;
            }

            if (value is string s)
            {
                if (s.ToLower() == "yes")
                {
                    return true;
                }
                else if (s.ToLower() == "no")
                {
                    return false;
                }
                else
                {
                    throw new ArgumentException($"Could not parse value '{s}' as boolean.");
                }
            }

            throw new ArgumentException($"Could not parse value '{value}' of type '{value.GetType().FullName}' as boolean.");
        }

        private static int? GetDbValueAsInt(object dbValue)
        {
            if (dbValue == null)
            {
                return null;
            }

            return int.Parse(dbValue.ToString());
        }

        #endregion

        #region Overridden

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactory.Instance;

        protected override List<ColumnInfo> GetColumnInfos()
        {
            using var command = this.Connection.CreateCommand();
            command.CommandText =
                @"
SELECT
    C.column_name               ColumnName,
    C.is_nullable               IsNullable,
    C.data_type                 DataType,
    C.character_maximum_length  MaxLen,
    C.numeric_precision         NumericPrecision,
    C.numeric_scale             NumericScale
FROM
    information_schema.columns C
WHERE
    C.table_name = @p_tableName
ORDER BY
    C.ordinal_position
";

            command.AddParameterWithValue("p_tableName", this.TableName);

            var columnInfos = DbTools
                .GetCommandRows(command)
                .Select(x => new ColumnInfo
                {
                    Name = x.ColumnName,
                    TypeName = x.DataType,
                    IsNullable = ParseBoolean(x.IsNullable),
                    Size = GetDbValueAsInt(x.MaxLen),
                    Precision = GetDbValueAsInt(x.NumericPrecision),
                    Scale = GetDbValueAsInt(x.NumericScale),
                })
                .ToList();

            return columnInfos;
        }

        protected override ColumnMold ColumnInfoToColumnMold(ColumnInfo columnInfo)
        {
            var column = new ColumnMold
            {
                Name = columnInfo.Name,
                Type = new DbTypeMold
                {
                    Name = columnInfo.TypeName,
                    Size = columnInfo.Size,
                    Precision = columnInfo.Precision,
                    Scale = columnInfo.Scale,
                },
                IsNullable = columnInfo.IsNullable,
            };

            if (columnInfo.TypeName == "integer") // todo: for all types which are indeed have not precision & scale.
            {
                column.Type.Precision = null;
                column.Type.Scale = null;
            }

            return column;
        }

        protected override Dictionary<string, ColumnIdentityMold> GetIdentities()
        {
            using var command = this.Connection.CreateCommand();
            command.CommandText =
@"
SELECT 
    S.column_name           ColumnName, 
    S.is_identity           IsIdentity,
    S.identity_start        SeedValue,
    S.identity_increment    IncrementValue
FROM 
    information_schema.columns S
WHERE 
    S.table_name = @p_tableName AND
    S.is_identity = 'YES'
";
            command.AddParameterWithValue("p_tableName", this.TableName);

            return DbTools
                .GetCommandRows(command)
                .ToDictionary(
                    x => (string)x.Name,
                    x => new ColumnIdentityMold
                    {
                        Seed = ((object)x.SeedValue).ToString(),
                        Increment = ((object)x.IncrementValue).ToString(),
                    });
        }

        public override PrimaryKeyMold GetPrimaryKey()
        {
            return NpgsqlTools.LoadPrimaryKey(this.Connection, this.Schema, this.TableName);
        }

        public override IReadOnlyList<ForeignKeyMold> GetForeignKeys()
        {
            using var command = this.Connection.CreateCommand();

            command.CommandText =
                @"
SELECT
    TC.constraint_name    ConstraintName
FROM
    information_schema.table_constraints TC
WHERE
    TC.constraint_schema = @p_schema AND
    TC.table_schema = @p_schema AND
    TC.table_name = @p_tableName AND
    TC.constraint_type = 'FOREIGN KEY'
";
            command.AddParameterWithValue("@p_schema", this.Schema);
            command.AddParameterWithValue("@p_tableName", this.TableName);

            var rows = DbTools.GetCommandRows(command);

            var fkNames = rows
                .Select(x => (string)x.ConstraintName)
                .ToList();

            return fkNames
                .Select(this.LoadForeignKey)
                .ToList();
        }

        private ForeignKeyMold LoadForeignKey(string fkName)
        {
            // get referenced table name
            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    CCU.table_name TableName
FROM
    information_schema.constraint_column_usage CCU
WHERE
    CCU.table_schema = @p_schema
    AND
    CCU.constraint_schema = @p_schema
    AND
    CCU.constraint_name = @p_fkName
";
            command.AddParameterWithValue("p_schema", this.Schema);
            command.AddParameterWithValue("p_fkName", fkName);

            var referencedTableName = DbTools.GetCommandRows(command)
                .Select(x => (string)x.TableName)
                .Distinct()
                .Single();

            // get referenced table PK
            var referencedTablePk = NpgsqlTools.LoadPrimaryKey(this.Connection, this.Schema, referencedTableName);

            // get foreign key columns

            command.Parameters.Clear();

            command.CommandText = @"
select
    KCU.column_name 					ColumnName,
    KCU.ordinal_position 				OrdinalPosition,
    KCU.position_in_unique_constraint 	PositionInUniqueConstraint
from
    information_schema.key_column_usage KCU	
where
    KCU.constraint_schema = @p_schema and
    KCU.table_schema = @p_schema and
    KCU.table_name = @p_tableName and
    KCU.constraint_name = @p_fkName
order by
    KCU.ordinal_position
";

            command.AddParameterWithValue("p_schema", this.Schema);
            command.AddParameterWithValue("p_tableName", this.TableName);
            command.AddParameterWithValue("p_fkName", fkName);

            var rows = DbTools.GetCommandRows(command);

            var columnNames = new List<string>();
            var referencedColumnNames = new List<string>();

            foreach (var row in rows)
            {
                var columnName = (string)row.ColumnName;
                var positionInUniqueConstraint = (int)row.PositionInUniqueConstraint;

                columnNames.Add(columnName);

                var referencedColumnName = referencedTablePk.Columns[positionInUniqueConstraint - 1].Name;

                referencedColumnNames.Add(referencedColumnName);
            }

            var fk = new ForeignKeyMold
            {
                Name = fkName,
                ReferencedTableName = referencedTableName,
                ColumnNames = columnNames,
                ReferencedColumnNames = referencedColumnNames,
            };

            return fk;
        }

        public override IReadOnlyList<IndexMold> GetIndexes()
        {
            using var command = this.Connection.CreateCommand();

            command.CommandText = @"
SELECT
    IX.indexname IndexName,
    IX.indexdef  IndexDef
FROM
    pg_indexes IX
WHERE
    IX.schemaname = @p_schema AND
    IX.tablename = @p_tableName
";
            command.AddParameterWithValue("p_schema", this.Schema);
            command.AddParameterWithValue("p_tableName", this.TableName);


            var rows = DbTools.GetCommandRows(command);

            return rows
                .Select(x => this.BuildIndexMold((string)x.IndexName, (string)x.IndexDef))
                .ToList();
        }

        private IndexMold BuildIndexMold(string indexName, string indexDefinition)
        {
            var isUnique = indexDefinition.StartsWith("CREATE UNIQUE", StringComparison.InvariantCultureIgnoreCase);

            var left = indexDefinition.IndexOf('(');
            var right = indexDefinition.IndexOf(')');

            var columnsString = indexDefinition.Substring(left + 1, right - left - 1);
            var columnDefinitions = columnsString.Split(',').Select(x => x.Trim()).ToList();

            var columns = new List<IndexColumnMold>();

            foreach (var columnDefinition in columnDefinitions)
            {
                string columnName;
                SortDirection sortDirection;
                if (columnDefinition.EndsWith(" DESC"))
                {
                    columnName = columnDefinition.Substring(0, columnDefinition.Length - " DESC".Length);
                    sortDirection = SortDirection.Descending;
                }
                else
                {
                    columnName = columnDefinition;
                    sortDirection = SortDirection.Ascending;
                }

                var indexColumnMold = new IndexColumnMold
                {
                    Name = columnName,
                    SortDirection = sortDirection,
                };

                columns.Add(indexColumnMold);
            }

            var indexMold = new IndexMold
            {
                Name = indexName,
                TableName = this.TableName,
                Columns = columns,
                IsUnique = isUnique,
            };

            return indexMold;
        }

        #endregion
    }
}
