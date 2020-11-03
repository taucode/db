using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Algorithms.Graphs;
using TauCode.Db;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.MySql
{
    public static class MySqlToolsLab
    {
        internal static readonly HashSet<string> SystemSchemata = new HashSet<string>(new[]
        {
            "mysql",
            "information_schema",
            "performance_schema",
        });

        public static IReadOnlyList<string> GetSchemata(this MySqlConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            using var command = connection.CreateCommand();
            command.CommandText = @"
SELECT
    S.schema_name SchemaName
FROM
    information_schema.schemata S
";

            var schemata = command
                .GetCommandRows()
                .Select(x => (string)x.SchemaName)
                .Except(SystemSchemata)
                .ToList();

            return schemata;
        }

        public static string GetSchemaName(this MySqlConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (connection.State != ConnectionState.Open)
            {
                throw new ArgumentException("Connection should be opened.", nameof(connection)); // todo: exception message is copy-pasted
            }

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT SCHEMA()";
            var schemaNameObject = command.ExecuteScalar();
            if (schemaNameObject == DBNull.Value)
            {
                return null;
            }

            return (string)schemaNameObject;
        }

        public static void CreateSchema(this MySqlConnection connection, string schemaName)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            using var command = connection.CreateCommand();
            command.CommandText = $"CREATE SCHEMA `{schemaName}`";
            command.ExecuteNonQuery();
        }

        public static IList<ForeignKeyMold> GetTableForeignKeys(
            this MySqlConnection connection,
            string schemaName,
            string tableName,
            bool loadColumns)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            using var command = connection.CreateCommand();
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
    TC.constraint_name = RC.constraint_name
INNER JOIN
    information_schema.table_constraints TC2
ON
    TC2.constraint_name = RC.unique_constraint_name AND TC2.constraint_type = 'PRIMARY KEY'
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

            command.Parameters.AddWithValue("@p_schemaName", schemaName);
            command.Parameters.AddWithValue("@p_tableName", tableName);

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
    CU.ordinal_position = CU2.ordinal_position
WHERE
    CU.constraint_name = @p_fkName AND
    CU.constraint_schema = @p_schemaName AND
    CU.table_schema = @p_schemaName AND

    CU2.CONSTRAINT_SCHEMA = @p_schemaName AND
    CU2.TABLE_SCHEMA = @p_schemaName
ORDER BY
    CU.ordinal_position
";

                command.Parameters.Clear();

                var fkParam = command.Parameters.Add("p_fkName", MySqlDbType.VarChar, 100);
                var schemaParam = command.Parameters.Add("p_schemaName", MySqlDbType.VarChar, 100);
                schemaParam.Value = schemaName;

                command.Prepare();

                foreach (var fk in foreignKeyMolds)
                {
                    fkParam.Value = fk.Name;

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

        public static IReadOnlyList<string> GetTableNames(
            this MySqlConnection connection,
            string schemaName,
            bool? independentFirst)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            using var command = connection.CreateCommand();
            command.CommandText = @"
SELECT
    T.table_name TableName
FROM
    information_schema.tables T
WHERE
    T.table_type = 'BASE TABLE' AND
    T.table_schema = @p_schemaName
ORDER BY
    T.table_name
";

            command.Parameters.AddWithValue("p_schemaName", schemaName);

            var tableNames = DbTools
                .GetCommandRows(command)
                .Select(x => (string)x.TableName)
                .ToList();

            if (independentFirst.HasValue)
            {
                var tableMolds = tableNames
                    .Select(x => new TableMold
                    {
                        Name = x,
                    })
                    .ToList();

                foreach (var tableMold in tableMolds)
                {
                    var foreignKeys = connection.GetTableForeignKeys(schemaName, tableMold.Name, false);
                    tableMold.ForeignKeys = foreignKeys;
                }

                var graph = new Graph<TableMold>();

                var dictionary = tableMolds.ToDictionary(
                    x => x.Name,
                    x => graph.AddNode(x));

                foreach (var tableMold in tableMolds)
                {
                    var tableNode = dictionary[tableMold.Name];

                    foreach (var foreignKey in tableMold.ForeignKeys)
                    {
                        var referencedTableNode = dictionary.GetValueOrDefault(foreignKey.ReferencedTableName);
                        if (referencedTableNode == null)
                        {
                            throw new TauDbException(
                                $"Table '{foreignKey.ReferencedTableName}' not found."); // todo: standard exception 'object not found' to avoid copy/paste
                        }

                        tableNode.DrawEdgeTo(referencedTableNode);
                    }
                }

                var algorithm = new GraphSlicingAlgorithm<TableMold>(graph);
                var slices = algorithm.Slice();

                if (!independentFirst.Value)
                {
                    slices = slices.Reverse().ToArray();
                }

                var arrangedTableNames = slices
                    .SelectMany(x => x.Nodes.OrderBy(y => y.Value.Name))
                    .Select(x => x.Value.Name)
                    .ToList();

                return arrangedTableNames;
            }

            return tableNames;
        }

        public static IReadOnlyList<TableMold> GetTableMolds(
            this MySqlConnection connection,
            string schemaName,
            bool? independentFirst)
        {
            var tableNames = GetTableNames(connection, schemaName, independentFirst);
            var inspector = new MySqlInspectorLab(connection);

            return tableNames
                .Select(x => inspector.Factory.CreateTableInspector(connection, schemaName, x).GetTable())
                .ToList();
        }

        public static void DropSchema(this MySqlConnection connection, string schemaName)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            using var command = connection.CreateCommand();
            command.CommandText = $"DROP SCHEMA `{schemaName}`";
            command.ExecuteNonQuery();
        }

        public static void DropTable(this MySqlConnection connection, string schemaName, string tableName)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            using var command = connection.CreateCommand();
            command.CommandText = $"DROP TABLE `{schemaName}`.`{tableName}`";
            command.ExecuteNonQuery();
        }

        public static bool SchemaExists(this MySqlConnection connection, string schemaName)
        {
            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            return GetSchemata(connection).Contains(schemaName); // todo optimize
        }
    }
}
