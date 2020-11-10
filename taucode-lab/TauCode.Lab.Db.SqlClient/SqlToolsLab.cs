using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
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

        public const int DefaultDecimalPrecision = 18;

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

            var schemata = command
                .GetCommandRows()
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

        //public static IReadOnlyList<TableMold> GetTableMolds(
        //    this SqlConnection connection,
        //    string schemaName,
        //    bool? independentFirst)
        //{
        //    var tableNames = GetTableNames(connection, schemaName, independentFirst);
        //    var inspector = new SqlInspectorLab(connection, schemaName);

        //    return tableNames
        //        .Select(x => inspector.Factory.CreateTableInspector(connection, schemaName, x).GetTable())
        //        .ToList();
        //}

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

                var fkParam = command.Parameters.Add("p_fkName", SqlDbType.NVarChar, 100);
                var schemaParam = command.Parameters.Add("p_schemaName", SqlDbType.NVarChar, 100);
                schemaParam.Value = schemaName;

                command.Prepare();

                foreach (var fk in foreignKeyMolds)
                {
                    fkParam.Value = fk.Name;

                    var rows = DbTools.GetCommandRows(command);

                    fk.ColumnNames = rows
                        .Select(x => (string) x.ColumnName)
                        .ToList();

                    fk.ReferencedColumnNames = rows
                        .Select(x => (string) x.ReferencedColumnName)
                        .ToList();
                }
            }

            return foreignKeyMolds;
        }

        public static IList<IndexMold> GetTableIndexes(
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
    I.[index_id]            IndexId,
    I.[name]                IndexName,
    I.[is_unique]           IndexIsUnique,
    IC.[key_ordinal]        KeyOrdinal,
    C.[name]                ColumnName,
    IC.[is_descending_key]  IsDescendingKey
FROM
    sys.indexes I
INNER JOIN
    sys.index_columns IC
ON
    IC.[index_id] = I.[index_id]
    AND
    IC.[object_id] = I.[object_id]
INNER JOIN
    sys.columns C
ON
    C.[column_id] = IC.[column_id]
    AND
    C.[object_id] = IC.[object_id]
INNER JOIN
    sys.tables T
ON
    T.[object_id] = C.[object_id]
INNER JOIN
    sys.schemas S
ON
    T.[schema_id] = S.[schema_id]
WHERE
    T.[name] = @p_tableName and
    S.[name] = @p_schemaName
ORDER BY
    I.[name],
    IC.[key_ordinal]
";

            command.Parameters.AddWithValue("p_tableName", tableName);
            command.Parameters.AddWithValue("p_schemaName", schemaName);

            return DbTools
                .GetCommandRows(command)
                .GroupBy(x => (int) x.IndexId)
                .Select(g => new IndexMold
                {
                    Name = (string) g.First().IndexName,
                    TableName = tableName,
                    Columns = g
                        .OrderBy(x => (int) x.KeyOrdinal)
                        .Select(x => new IndexColumnMold
                        {
                            Name = (string) x.ColumnName,
                            SortDirection = (bool) x.IsDescendingKey
                                ? SortDirection.Descending
                                : SortDirection.Ascending,
                        })
                        .ToList(),
                    IsUnique = (bool) g.First().IndexIsUnique,
                })
                .OrderBy(x => x.Name)
                .ToList();
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

        public static bool SchemaExists(this SqlConnection connection, string schemaName)
        {
            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            return GetSchemata(connection).Contains(schemaName); // todo optimize
        }

        public static bool TableExists(this SqlConnection connection, string schemaName, string tableName)
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
    T.table_name
FROM
    information_schema.tables T
WHERE
    T.table_schema = @p_schemaName AND
    T.table_name = @p_tableName
";
            command.Parameters.AddWithValue("p_schemaName", schemaName);
            command.Parameters.AddWithValue("p_tableName", tableName);

            using var reader = command.ExecuteReader();
            return reader.Read();
        }

        public static PrimaryKeyMold GetTablePrimaryKey(
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
    TC.constraint_name ConstraintName,
    KCU.column_name ColumnName
FROM
    information_schema.table_constraints TC
INNER JOIN
    information_schema.key_column_usage KCU
ON
    KCU.constraint_name = TC.constraint_name
WHERE
    TC.CONSTRAINT_SCHEMA = @p_schemaName AND
    TC.TABLE_SCHEMA = @p_schemaName AND
    TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND
    TC.TABLE_NAME = @p_tableName AND

    KCU.CONSTRAINT_SCHEMA = @p_schemaName AND
    KCU.TABLE_SCHEMA = @p_schemaName
ORDER BY
    KCU.ordinal_position
";
            command.Parameters.AddWithValue("p_schemaName", schemaName);
            command.Parameters.AddWithValue("p_tableName", tableName);

            var rows = DbTools.GetCommandRows(command);

            if (rows.Count == 0)
            {
                return null; // no PK
            }

            var constraintName = (string) rows[0].ConstraintName;
            var columnNames = rows
                .Select(x => (string) x.ColumnName)
                .ToList();

            var pk = new PrimaryKeyMold
            {
                Name = constraintName,
                Columns = columnNames,
            };

            return pk;
        }
    }
}
