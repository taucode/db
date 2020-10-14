using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Algorithms.Graphs;
using TauCode.Db;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;

// todo: remove unused methods
namespace TauCode.Lab.Db.SqlClient
{
    public static class SqlToolsLab
    {
        public const string DefaultSchemaName = "dbo";

        internal static readonly HashSet<string> SystemSchemata = new HashSet<string>(new[]
        {
            "guest",
            "INFORMATION_SCHEMA",
            "sys",
            "db_owner",
            "db_accessadmin",
            "db_securityadmin",
            "db_ddladmin",
            "db_backupoperator",
            "db_datareader",
            "db_datawriter",
            "db_denydatareader",
            "db_denydatawriter",
        });

        public static IReadOnlyList<string> GetSchemata(this SqlConnection connection)
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

            var schemata = DbTools
                .GetCommandRows(command)
                .Select(x => (string) x.SchemaName)
                .Except(SystemSchemata)
                .ToList();

            return schemata;
        }

        public static IReadOnlyList<string> GetTableNames(
            this SqlConnection connection,
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
                .Select(x => (string) x.TableName)
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

        public static void DropTable(this SqlConnection connection, string schemaName, string tableName)
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
            command.CommandText = $"DROP TABLE [{schemaName}].[{tableName}]";
            command.ExecuteNonQuery();
        }

        public static void DropSchema(this SqlConnection connection, string schemaName)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            if (string.Equals(schemaName, DefaultSchemaName, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new TauDbException($"Cannot drop default schema '{DefaultSchemaName}'.");
            }

            using var command = connection.CreateCommand();
            command.CommandText = $"DROP SCHEMA [{schemaName}]";
            command.ExecuteNonQuery();
        }

        public static void CreateSchema(this SqlConnection connection, string schemaName)
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
            command.CommandText = $@"CREATE SCHEMA [{schemaName}]";
            command.ExecuteNonQuery();
        }

        public static IList<ForeignKeyMold> GetTableForeignKeys(
            this SqlConnection connection,
            string schemaName,
            string tableName,
            bool loadColumns)
        {
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

            var foreignKeyMolds = DbTools
                .GetCommandRows(command)
                .Select(x => new ForeignKeyMold
                {
                    Name = x.ConstraintName,
                    ReferencedTableName = x.ReferencedTableName,
                })
                .ToList();

            if (loadColumns)
            {
                throw new NotImplementedException();
            }

            return foreignKeyMolds;
        }

        public static IReadOnlyList<string> GetTableForeignKeyNames(
            this SqlConnection connection,
            string schemaName,
            string tableName)
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
    TC.constraint_name ConstraintName
FROM
    information_schema.table_constraints TC
WHERE
    TC.constraint_schema = @p_schemaName AND
    TC.table_schema = @p_schemaName AND
    TC.table_name = @p_tableName AND
    TC.constraint_type = 'FOREIGN KEY'
";

            command.Parameters.AddWithValue("p_schemaName", schemaName);
            command.Parameters.AddWithValue("p_tableName", tableName);

            var names = DbTools
                .GetCommandRows(command)
                .Select(x => (string) x.ConstraintName)
                .ToList();

            return names;
        }

        public static void DropForeignKey(
            this SqlConnection connection,
            string schemaName,
            string tableName,
            string foreignKeyName)
        {
            throw new NotImplementedException();
        }
    }
}
