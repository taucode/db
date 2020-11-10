using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Model;
using TauCode.Db.Schema;
using TauCode.Extensions;

namespace TauCode.Lab.Db.Npgsql
{
    // todo: copy-pasted a lot from SQL Server.
    public class NpgsqlTableInspectorLab : DbTableInspectorBase
    {
        public NpgsqlTableInspectorLab(NpgsqlConnection connection, string schemaName, string tableName)
            : base(
                connection,
                schemaName ?? NpgsqlToolsLab.DefaultSchemaName,
                tableName)
        {
            this.SchemaExplorer = new NpgsqlSchemaExplorer(this.NpgsqlConnection);
        }

        private static int? GetDbValueAsInt(object dbValue)
        {
            if (dbValue == null)
            {
                return null;
            }

            return int.Parse(dbValue.ToString());
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

        protected IDbSchemaExplorer SchemaExplorer { get; }

        protected NpgsqlConnection NpgsqlConnection => (NpgsqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactoryLab.Instance;

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
    C.table_name = @p_tableName AND
    C.table_schema = @p_schema
ORDER BY
    C.ordinal_position
";

            command.AddParameterWithValue("p_tableName", this.TableName);
            command.AddParameterWithValue("p_schema", this.SchemaName);

            var columnInfos = command
                .GetCommandRows()
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

            if (column.Type.Name.IsIn(
                "smallint",
                "integer",
                "bigint",
                "double precision",
                "real"))
            {
                column.Type.Precision = null;
                column.Type.Scale = null;
            }

            return column;
        }

        public override IReadOnlyList<ColumnMold> GetColumns()
        {
            return this
                .SchemaExplorer
                .GetTableColumns(this.SchemaName, this.TableName, true);
        }

        protected override Dictionary<string, ColumnIdentityMold> GetIdentities()
        {
            throw new NotImplementedException();
//            using var command = (NpgsqlCommand)this.Connection.CreateCommand();
//            command.CommandText = @"
//SELECT
//    C.column_name           ColumnName,
//    C.identity_start        Seed,
//    C.identity_increment    TheIncrement
//FROM
//    information_schema.columns C
//WHERE
//    C.table_name = @p_tableName AND
//    C.table_schema = @p_schemaName AND
//    C.is_identity = 'YES'
//";

//            command.Parameters.AddWithValue("p_tableName", this.TableName);
//            command.Parameters.AddWithValue("p_schemaName", this.SchemaName);

//            var rows = command.GetCommandRows();
//            if (rows.Count != 0)
//            {
//                return rows
//                    .ToDictionary(
//                        x => (string)x.ColumnName,
//                        x => new ColumnIdentityMold
//                        {
//                            Seed = x.Seed.ToString(),
//                            Increment = x.TheIncrement.ToString(),
//                        });
//            }

//            return new Dictionary<string, ColumnIdentityMold>();
        }

        public override IReadOnlyList<ForeignKeyMold> GetForeignKeys()
        {
            return this
                .SchemaExplorer
                .GetTableForeignKeys(this.SchemaName, this.TableName, true, true);
        }

        public override PrimaryKeyMold GetPrimaryKey()
        {
            return this.SchemaExplorer
                .GetTablePrimaryKey(this.SchemaName, this.TableName, true);
        }

        public override IReadOnlyList<IndexMold> GetIndexes()
        {
            return this.SchemaExplorer
                .GetTableIndexes(this.SchemaName, this.TableName, true);
        }

        protected override bool NeedCheckSchemaExistence => throw new NotImplementedException();

        protected override bool SchemaExists(string schemaName) => throw new NotImplementedException();

        protected override bool TableExists(string tableName) => throw new NotImplementedException();

        protected override PrimaryKeyMold GetPrimaryKeyImpl() =>
            //this.NpgsqlConnection.GetTablePrimaryKey(this.SchemaName, this.TableName);
            throw new NotImplementedException();

        protected override IReadOnlyList<ForeignKeyMold> GetForeignKeysImpl()
            //=> this.NpgsqlConnection.GetTableForeignKeys(this.SchemaName, this.TableName, true).ToList();
            => throw new NotImplementedException();

        protected override IReadOnlyList<IndexMold> GetIndexesImpl()
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
            command.AddParameterWithValue("p_schema", this.SchemaName);
            command.AddParameterWithValue("p_tableName", this.TableName);


            var rows = command.GetCommandRows();

            return rows
                .Select(x => this.BuildIndexMold((string)x.IndexName, (string)x.IndexDef))
                .OrderBy(x => x.Name)
                .ToList();
        }

        public override TableMold GetTable() => this.SchemaExplorer.GetTable(
            this.SchemaName,
            this.TableName,
            true,
            true,
            true,
            true);

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
                    Name = columnName.Replace("\"", "", StringComparison.InvariantCultureIgnoreCase),
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
    }
}
