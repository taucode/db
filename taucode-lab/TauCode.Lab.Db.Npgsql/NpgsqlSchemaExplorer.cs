using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Model;
using TauCode.Db.Schema;
using TauCode.Extensions;

namespace TauCode.Lab.Db.Npgsql
{
    public class NpgsqlSchemaExplorer : DbSchemaExplorerBase
    {
        public NpgsqlSchemaExplorer(NpgsqlConnection connection)
            : base(connection, "\"\"")
        {
        }

        protected override ColumnMold ColumnInfoToColumn(ColumnInfo columnInfo)
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

        protected IndexMold BuildIndexMold(string tableName, string indexName, string indexDefinition)
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
                TableName = tableName,
                Columns = columns,
                IsUnique = isUnique,
            };

            return indexMold;
        }

        protected override IReadOnlyList<IndexMold> GetTableIndexesImpl(string schemaName, string tableName)
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
            command.AddParameterWithValue("p_schema", schemaName);
            command.AddParameterWithValue("p_tableName", tableName);

            var rows = command.GetCommandRows();

            return rows
                .Select(x => this.BuildIndexMold(
                    tableName,
                    (string)x.IndexName,
                    (string)x.IndexDef))
                .OrderBy(x => x.Name)
                .ToList();
        }

        protected override IList<string> GetAdditionalColumnTableColumnNames()
        {
            // todo use 'static constant', here & anywhere
            return new List<string>()
            {
                "identity_start",
                "identity_increment",
            };
        }

        protected override void ResolveIdentities(string schemaName, string tableName, IList<ColumnInfo> columnInfos)
        {
            foreach (var columnInfo in columnInfos)
            {
                if (columnInfo.Additional.ContainsKey("identity_start"))
                {
                    columnInfo.Additional["#identity_seed"] = columnInfo.Additional["identity_start"];
                    columnInfo.Additional["#identity_increment"] =
                        columnInfo.Additional.GetValueOrDefault("identity_start");
                }
            }
        }

        public override IReadOnlyList<string> GetSystemSchemata()
        {
            return new HashSet<string>(new[]
            {
                "information_schema",
                "pg_catalog",
                "pg_toast",
            }).ToList(); // todo
        }

        public override string DefaultSchemaName => "public";
    }
}
