using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Model;
using TauCode.Extensions;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlTableInspector : DbTableInspectorBase
    {
        #region Constructor

        public MySqlTableInspector(IDbConnection connection, string tableName)
            : base(
                connection,
                null,
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

        public override IDbUtilityFactory Factory => MySqlUtilityFactory.Instance;

        protected override List<ColumnInfo> GetColumnInfos()
        {
            var dbName = this.Connection.GetDatabaseName();

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
    C.table_name = @p_tableName AND
    C.table_schema = @p_dbName    
ORDER BY
    C.ordinal_position
";

            command.AddParameterWithValue("p_tableName", this.TableName);
            command.AddParameterWithValue("p_dbName", dbName);

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

            if (column.Type.Name.ToLowerInvariant().IsIn(
                "bit",
                "int"))
            {
                column.Type.Precision = null;
                column.Type.Scale = null;
            }

            if (column.Type.Name.ToLowerInvariant().IsIn(
                "text"))
            {
                column.Type.Size = null;
            }

            return column;
        }

        protected override Dictionary<string, ColumnIdentityMold> GetIdentities()
        {
            var dbName = this.Connection.GetDatabaseName();

            using var command = this.Connection.CreateCommand();

            // find out if table supports auto_increment

            command.CommandText =
@"
SELECT
    T.auto_increment    AutoIncrement
FROM
    information_schema.tables T
WHERE
    T.table_name = @p_tableName
    AND
    T.table_type = @p_tableType
    AND
    T.table_schema = @p_dbName
";
            command.AddParameterWithValue("p_tableName", this.TableName);
            command.AddParameterWithValue("p_tableType", MySqlTools.TableTypeForTable);
            command.AddParameterWithValue("p_dbName", dbName);

            var rows = DbTools.GetCommandRows(command);

            if (rows.Count == 0)
            {
                throw DbTools.CreateTableNotFoundException(this.TableName);
            }

            int? autoIncrement = (int?)rows.Single().AutoIncrement;

            if (autoIncrement.HasValue)
            {
                command.Parameters.Clear();

                command.CommandText = @"
SELECT 
    S.column_name ColumnName
FROM 
    information_schema.columns S
WHERE
    S.table_schema = @p_dbName
    AND
    S.table_name = @p_tableName
    AND
    S.extra like '%auto_increment%'
";

                command.AddParameterWithValue("p_dbName", dbName);
                command.AddParameterWithValue("p_tableName", this.TableName);

                var rows2 = DbTools.GetCommandRows(command);
                var columnName = (string)rows2.Single().ColumnName;

                return new Dictionary<string, ColumnIdentityMold>
                {
                    {
                        columnName,
                        new ColumnIdentityMold
                        {
                            Seed = autoIncrement.Value.ToString(),
                            Increment = (1).ToString(),
                        }
                    },
                };
            }
            else
            {
                return new Dictionary<string, ColumnIdentityMold>();
            }
        }

        public override PrimaryKeyMold GetPrimaryKey()
        {
            return MySqlTools.LoadPrimaryKey(this.Connection, this.TableName);
        }

        public override IReadOnlyList<ForeignKeyMold> GetForeignKeys()
        {
            var dbName = this.Connection.GetDatabaseName();

            using var command = this.Connection.CreateCommand();

            // get constraint names
            command.CommandText =
@"
SELECT
    TC.constraint_name                  ConstraintName,
    KCU.column_name                     ColumnName,
    KCU.referenced_table_name           ReferencedTableName,
    KCU.referenced_column_name          ReferencedColumnName,
    KCU.ordinal_position                OrdinalPosition,
    KCU.position_in_unique_constraint   PositionInUniqueConstraint
FROM
    information_schema.table_constraints TC
INNER JOIN
    information_schema.key_column_usage KCU
ON
    TC.constraint_name = KCU.constraint_name
WHERE
    TC.constraint_schema = @p_dbName AND
    TC.table_schema = @p_dbName AND

    KCU.constraint_schema = @p_dbName AND
    KCU.table_schema = @p_dbName AND
    KCU.referenced_table_schema = @p_dbName AND

    TC.table_name = @p_tableName AND
    TC.constraint_type = 'FOREIGN KEY'
ORDER BY
    TC.constraint_name,
    KCU.ordinal_position
";
            command.AddParameterWithValue("@p_dbName", dbName);
            command.AddParameterWithValue("@p_tableName", this.TableName);

            var rows = DbTools.GetCommandRows(command);

            return rows
                .GroupBy(x => (string)x.ConstraintName)
                .Select(x => new ForeignKeyMold
                {
                    Name = x.Key,
                    ColumnNames = x.Select(y => (string)y.ColumnName).ToList(),
                    ReferencedTableName = (string)x.First().ReferencedTableName,
                    ReferencedColumnNames = x.Select(y => (string)y.ReferencedColumnName).ToList(),

                })
                .ToList();
        }

        public override IReadOnlyList<IndexMold> GetIndexes()
        {
            var dbName = this.Connection.GetDatabaseName();

            using var command = this.Connection.CreateCommand();

            command.CommandText = @"
SELECT
    S.index_name    IndexName,
    S.non_unique    NonUnique,
    S.column_name   ColumnName,
    S.seq_in_index  SeqInIndex
FROM
    information_schema.statistics S
WHERE
    S.table_schema = @p_dbName
    AND
    S.index_schema = @p_dbName
    AND
    S.table_name = @p_tableName
";
            command.AddParameterWithValue("p_dbName", dbName);
            command.AddParameterWithValue("p_tableName", this.TableName);

            var rows = DbTools.GetCommandRows(command);

            return rows
                .GroupBy(x => (string)x.IndexName)
                .Select(x => new IndexMold
                {
                    Name = x.Key,
                    TableName = this.TableName,
                    Columns = x
                        .OrderBy(y => (int)y.SeqInIndex)
                        .Select(y => new IndexColumnMold
                        {
                            Name = (string)y.ColumnName,
                            SortDirection = SortDirection.Ascending,
                        })
                        .ToList(),
                    IsUnique = x.First().NonUnique.ToString() == "0",
                })
                .ToList();
        }

        #endregion
    }
}
