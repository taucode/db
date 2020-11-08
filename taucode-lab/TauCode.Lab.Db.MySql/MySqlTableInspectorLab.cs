using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;
using TauCode.Extensions;

namespace TauCode.Lab.Db.MySql
{
    // todo: a lot of copy-paste
    // todo clean up
    public class MySqlTableInspectorLab : DbTableInspectorBase
    {
        public MySqlTableInspectorLab(MySqlConnection connection, string tableName)
            : base(connection, connection.GetSchemaName(), tableName)
        {
        }

        protected MySqlConnection MySqlConnection => (MySqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => MySqlUtilityFactoryLab.Instance;

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
    C.numeric_scale             NumericScale,
    C.character_set_name        CharacterSetName,
    C.collation_name            CollationName,
    C.column_type               ColumnType
FROM
    information_schema.columns C
WHERE
    C.table_name = @p_tableName AND
    C.table_schema = @p_schema
ORDER BY
    C.ordinal_position
";

            command.AddParameterWithValue("p_tableName", this.TableName);
            command.AddParameterWithValue("p_schema", this.SchemaName);

            var rows = command.GetCommandRows();
            var columnInfos = new List<ColumnInfo>();

            foreach (var row in rows)
            {
                var columnInfo = new ColumnInfo
                {
                    Name = row.ColumnName,
                    TypeName = row.DataType,
                    IsNullable = ParseBoolean(row.IsNullable),
                    Size = GetDbValueAsInt(row.MaxLen),
                    Precision = GetDbValueAsInt(row.NumericPrecision),
                    Scale = GetDbValueAsInt(row.NumericScale),
                };

                if (row.CharacterSetName != null)
                {
                    columnInfo.TypeProperties["character_set_name"] = (string)row.CharacterSetName;
                }

                if (row.CollationName != null)
                {
                    columnInfo.TypeProperties["collation_name"] = (string)row.CollationName;
                }

                if (((string)row.ColumnType).EndsWith(" unsigned"))
                {
                    columnInfo.TypeProperties["unsigned"] = "true";
                }

                columnInfos.Add(columnInfo);
            }
            
            return columnInfos;
        }

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

            throw new ArgumentException(
                $"Could not parse value '{value}' of type '{value.GetType().FullName}' as boolean.");
        }

        private static int? GetDbValueAsInt(object dbValue)
        {
            if (dbValue == null)
            {
                return null;
            }

            var longValue = long.Parse(dbValue.ToString());
            if (longValue > int.MaxValue)
            {
                return null;
            }

            return (int)longValue;
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

            if (column.Type.Name.IsIn(
                "tinytext",
                "text",
                "mediumtext",

                "tinyblob",
                "blob",
                "mediumblob"))
            {
                column.Type.Size = null;
            }

            if (column.Type.Name.IsIn(
                "tinyint",
                "smallint",
                "int",
                "mediumint",
                "bigint",
                "float",
                "double"))
            {
                column.Type.Precision = null;
                column.Type.Scale = null;
            }

            column.Properties = columnInfo.ColumnProperties;
            column.Type.Properties = columnInfo.TypeProperties;

            return column;
        }

        protected override Dictionary<string, ColumnIdentityMold> GetIdentities()
        {
            using var command = this.Connection.CreateCommand();

            command.CommandText = @"
SELECT 
    S.column_name ColumnName
FROM 
    information_schema.columns S
WHERE
    S.table_schema = @p_schemaName
    AND
    S.table_name = @p_tableName
    AND
    S.extra like '%auto_increment%'
";

            command.AddParameterWithValue("p_schemaName", this.SchemaName);
            command.AddParameterWithValue("p_tableName", this.TableName);

            var rows = command.GetCommandRows();
            var columnName = (string)rows.SingleOrDefault()?.ColumnName;

            if (columnName == null)
            {
                return new Dictionary<string, ColumnIdentityMold>();
            }
            else
            {
                return new Dictionary<string, ColumnIdentityMold>
                {
                    {
                        columnName,
                        new ColumnIdentityMold
                        {
                            Seed = (1).ToString(),
                            Increment = (1).ToString(),
                        }
                    },
                };
            }
        }

        protected override bool NeedCheckSchemaExistence => true;

        protected override bool SchemaExists(string schemaName) => this.MySqlConnection.SchemaExists(schemaName);

        protected override bool TableExists(string tableName) => this.MySqlConnection.TableExists(tableName);

        protected override PrimaryKeyMold GetPrimaryKeyImpl()
        {
            using var command = this.Connection.CreateCommand();

            command.CommandText =
                @"
SELECT
    TC.constraint_name    ConstraintName,
    KCU.column_name       ColumnName,
    KCU.ordinal_position  OrdinalPosition
FROM
    information_schema.table_constraints TC
INNER JOIN
    information_schema.key_column_usage KCU
ON
    KCU.table_name = TC.table_name AND
    KCU.constraint_name = TC.constraint_name
WHERE
    TC.constraint_schema = @p_schemaName AND
    TC.table_schema = @p_schemaName AND

    KCU.constraint_schema = @p_schemaName AND
    KCU.table_schema = @p_schemaName AND

    TC.table_name = @p_tableName AND
    TC.constraint_type = 'PRIMARY KEY'
ORDER BY
    KCU.ordinal_position
";
            command.AddParameterWithValue("p_schemaName", this.SchemaName);
            command.AddParameterWithValue("p_tableName", this.TableName);

            var rows = command.GetCommandRows();
            if (rows.Count == 0)
            {
                return null;
            }

            var pkName = rows[0].ConstraintName;
            var pk = new PrimaryKeyMold
            {
                Name = pkName,
                Columns = rows
                    .Select(x => (string)x.ColumnName)
                    .ToList(),
            };

            return pk;
        }

        protected override IReadOnlyList<ForeignKeyMold> GetForeignKeysImpl()
            => this.MySqlConnection.GetTableForeignKeys(this.SchemaName, this.TableName, true).ToList();

        protected override IReadOnlyList<IndexMold> GetIndexesImpl()
        {
            using var command = this.Connection.CreateCommand();

            command.CommandText = @"
SELECT
    S.index_name    IndexName,
    S.table_name    TableName,
    S.non_unique    NonUnique,
    S.column_name   ColumnName,
    S.seq_in_index  SeqInIndex,
    S.collation     Collation
FROM
    information_schema.statistics S
WHERE
    S.table_schema = @p_schemaName
    AND
    S.index_schema = @p_schemaName
    AND
    S.table_name = @p_tableName
";
            command.AddParameterWithValue("p_schemaName", this.SchemaName);
            command.AddParameterWithValue("p_tableName", this.TableName);

            var rows = command.GetCommandRows();

            return rows
                .GroupBy(x => (string)x.IndexName)
                .Select(x => new IndexMold
                {
                    Name = x.Key,
                    TableName = (string)x.First().TableName,
                    Columns = x
                        .OrderBy(y => (int)y.SeqInIndex)
                        .Select(y => new IndexColumnMold
                        {
                            Name = (string)y.ColumnName,
                            SortDirection = CollationToSortDirection(y.Collation),
                        })
                        .ToList(),
                    IsUnique = x.First().NonUnique.ToString() == "0",
                })
                .ToList();
        }

        private SortDirection CollationToSortDirection(string collation)
        {
            switch (collation)
            {
                case "A":
                    return SortDirection.Ascending;

                case "D":
                    return SortDirection.Descending;

                default:
                    throw new TauDbException($"Unexpected index collation: '{collation}'.");
            }
        }
    }
}
