using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using TauCode.Db.Exceptions;
using TauCode.Db.Extensions;
using TauCode.Db.Model;
using TauCode.Extensions;

namespace TauCode.Db.MySql
{
    public class MySqlSchemaExplorer : DbSchemaExplorerBase
    {
        public MySqlSchemaExplorer(MySqlConnection connection)
            : base(connection, "``")
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

            var characterSetName = columnInfo.Additional.GetValueOrDefault("character_set_name");
            if (characterSetName != null)
            {
                column.Type.Properties["character_set_name"] = characterSetName;
            }

            var collationName = columnInfo.Additional.GetValueOrDefault("collation_name");
            if (collationName != null)
            {
                column.Type.Properties["collation_name"] = collationName;
            }

            var columnType = columnInfo.Additional["column_type"];
            if (columnType != null && columnType.EndsWith(" unsigned"))
            {
                column.Type.Properties["unsigned"] = "true";
            }

            var extra = columnInfo.Additional["extra"];
            if (!string.IsNullOrEmpty(extra) && extra.StartsWith("auto_increment"))
            {
                column.Identity = new ColumnIdentityMold("1", "1");
            }

            return column;
        }

        protected override IReadOnlyList<IndexMold> GetTableIndexesImpl(string schemaName, string tableName)
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
            command.AddParameterWithValue("p_schemaName", schemaName);
            command.AddParameterWithValue("p_tableName", tableName);

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

        protected override void ResolveIdentities(string schemaName, string tableName, IList<ColumnInfo> columnInfos)
        {
            // idle
        }

        public override IReadOnlyList<string> GetSystemSchemata()
        {

            return new List<string>
            {
                "mysql",
                "information_schema",
                "performance_schema",
            };
        }

        public override IReadOnlyList<ForeignKeyMold> GetTableForeignKeys(string schemaName, string tableName, bool loadColumns, bool checkExistence)
        {
            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (checkExistence)
            {
                this.CheckSchemaAndTableExistence(schemaName, tableName);
            }

            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    TC.constraint_name ConstraintName,
    RC.unique_constraint_name UniqueConstraintName,
    TC2.table_name ReferencedTableName
FROM
    information_schema.table_constraints TC
INNER JOIN
    information_schema.referential_constraints RC
ON
    TC.constraint_name = RC.constraint_name AND
    TC.table_name = RC.table_name
INNER JOIN
    information_schema.table_constraints TC2
ON
    TC2.constraint_name = RC.unique_constraint_name AND
    TC2.table_name = RC.referenced_table_name AND
    TC2.constraint_type = 'PRIMARY KEY'
WHERE
    TC.table_name = @p_tableName AND
    TC.constraint_type = 'FOREIGN KEY' AND

    TC.constraint_schema = @p_schemaName AND
    TC.table_schema = @p_schemaName AND

    RC.constraint_schema = @p_schemaName AND
    RC.unique_constraint_schema = @p_schemaName AND

    TC2.constraint_schema = @p_schemaName AND
    TC2.table_schema = @p_schemaName
";

            command.AddParameterWithValue("@p_schemaName", schemaName);
            command.AddParameterWithValue("@p_tableName", tableName);

            var foreignKeyMolds = command
                .GetCommandRows()
                .Select(x => new ForeignKeyMold
                {
                    Name = x.ConstraintName,
                    ReferencedTableName = x.ReferencedTableName,
                })
                .ToList();

            if (loadColumns)
            {
                command.CommandText = @"
SELECT
    CU.constraint_name  ConstraintName,
    CU.column_name      ColumnName,
    CU2.column_name     ReferencedColumnName
FROM
    information_schema.key_column_usage CU
INNER JOIN
    information_schema.referential_constraints RC
ON
    CU.constraint_name = RC.constraint_name
INNER JOIN
    information_schema.key_column_usage CU2
ON
    RC.unique_constraint_name = CU2.constraint_name AND
    CU.ordinal_position = CU2.ordinal_position AND
    CU2.table_name = @p_referencedTableName
WHERE
    CU.constraint_name = @p_fkName AND
    CU.constraint_schema = @p_schemaName AND
    CU.table_schema = @p_schemaName AND

    CU2.constraint_schema = @p_schemaName AND
    CU2.table_schema = @p_schemaName
ORDER BY
    CU.ordinal_position
";

                command.Parameters.Clear();

                var fkParam = command.CreateParameter();
                fkParam.ParameterName = "p_fkName";
                fkParam.DbType = DbType.String;
                fkParam.Size = 100;
                command.Parameters.Add(fkParam);

                var schemaParam = command.CreateParameter();
                schemaParam.ParameterName = "p_schemaName";
                schemaParam.DbType = DbType.String;
                schemaParam.Size = 100;
                command.Parameters.Add(schemaParam);

                schemaParam.Value = schemaName;

                var referencedTableNameParam = command.CreateParameter();
                referencedTableNameParam.ParameterName = "p_referencedTableName";
                referencedTableNameParam.DbType = DbType.String;
                referencedTableNameParam.Size = 100;
                command.Parameters.Add(referencedTableNameParam);

                schemaParam.Value = schemaName;

                command.Prepare();

                foreach (var fk in foreignKeyMolds)
                {
                    fkParam.Value = fk.Name;
                    referencedTableNameParam.Value = fk.ReferencedTableName;

                    var rows = command.GetCommandRows();

                    fk.ColumnNames = rows
                        .Select(x => (string)x.ColumnName)
                        .ToList();

                    fk.ReferencedColumnNames = rows
                        .Select(x => (string)x.ReferencedColumnName)
                        .ToList();
                }
            }

            return foreignKeyMolds;
        }

        protected override IList<string> GetAdditionalColumnTableColumnNames()
        {
            return new List<string>()
            {
                "character_set_name",
                "collation_name",
                "column_type",
                "extra",
            };
        }

        public override PrimaryKeyMold GetTablePrimaryKey(string schemaName, string tableName, bool checkExistence)
        {
            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (checkExistence)
            {
                this.CheckSchemaAndTableExistence(schemaName, tableName);
            }

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
            command.AddParameterWithValue("p_schemaName", schemaName);
            command.AddParameterWithValue("p_tableName", tableName);

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

        public override string DefaultSchemaName => null;

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
