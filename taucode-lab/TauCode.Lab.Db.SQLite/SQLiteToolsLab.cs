using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;
using TauCode.Extensions;

namespace TauCode.Lab.Db.SQLite
{
    public static class SQLiteToolsLab
    {
        /// <summary>
        /// Boosts SQLite insertions
        /// https://stackoverflow.com/questions/3852068/sqlite-insert-very-slow
        /// </summary>
        /// <param name="sqLiteConnection">SQLite connection to boost</param>
        public static void BoostSQLiteInsertions(this SQLiteConnection sqLiteConnection)
        {
            if (sqLiteConnection == null)
            {
                throw new ArgumentNullException(nameof(sqLiteConnection));
            }

            using var command = sqLiteConnection.CreateCommand();
            command.CommandText = "PRAGMA journal_mode = WAL";
            command.ExecuteNonQuery();

            command.CommandText = "PRAGMA synchronous = NORMAL";
            command.ExecuteNonQuery();
        }

        // todo: move this to taucode.db
        /// <summary>
        /// Creates temporary .sqlite file and returns a SQLite connection string for this file.
        /// </summary>
        /// <returns>
        /// Tuple with two strings. Item1 is temporary file path, Item2 is connection string.
        /// </returns>
        public static Tuple<string, string> CreateSQLiteDatabase()
        {
            var tempDbFilePath = FileTools.CreateTempFilePath("zunit", ".sqlite");
            SQLiteConnection.CreateFile(tempDbFilePath);

            var connectionString = $"Data Source={tempDbFilePath};Version=3;";

            return Tuple.Create(tempDbFilePath, connectionString);
        }

        public static string GetTableCreationSqlFromDb(SQLiteConnection connection, string tableName)
        {
            using var command = connection.CreateCommand();
            command.CommandText =
                @"
SELECT
    T.name  Name,
    T.sql   Sql
FROM
    sqlite_master T
WHERE
    T.type = @p_type
    AND
    T.name = @p_tableName
";

            command.Parameters.AddWithValue("p_type", "table");
            command.Parameters.AddWithValue("p_tableName", tableName);

            var rows = command.GetCommandRows();
            if (rows.Count == 0)
            {
                throw DbTools.CreateTableDoesNotExistException(null, tableName);
            }

            if (rows.Count > 1)
            {
                throw new TauDbException($"Internal error: more than one metadata row returned.");
            }

            return rows
                .Single()
                .Sql;
        }

        public static IReadOnlyList<string> GetTableNames(
            this SQLiteConnection connection,
            bool? independentFirst)
        {
            throw new NotImplementedException();

//            if (connection == null)
//            {
//                throw new ArgumentNullException(nameof(connection));
//            }

//            if (schemaName == null)
//            {
//                throw new ArgumentNullException(nameof(schemaName));
//            }

//            using var command = connection.CreateCommand();
//            command.CommandText = @"
//SELECT
//    T.table_name TableName
//FROM
//    information_schema.tables T
//WHERE
//    T.table_type = 'BASE TABLE' AND
//    T.table_schema = @p_schemaName
//ORDER BY
//    T.table_name
//";

//            command.Parameters.AddWithValue("p_schemaName", schemaName);

//            var tableNames = command
//                .GetCommandRows()
//                .Select(x => (string)x.TableName)
//                .ToList();

//            if (independentFirst.HasValue)
//            {
//                var tableMolds = tableNames
//                    .Select(x => new TableMold
//                    {
//                        Name = x,
//                    })
//                    .ToList();

//                foreach (var tableMold in tableMolds)
//                {
//                    var foreignKeys = connection.GetTableForeignKeys(schemaName, tableMold.Name, false);
//                    tableMold.ForeignKeys = foreignKeys;
//                }

//                var graph = new Graph<TableMold>();

//                var dictionary = tableMolds.ToDictionary(
//                    x => x.Name,
//                    x => graph.AddNode(x));

//                foreach (var tableMold in tableMolds)
//                {
//                    var tableNode = dictionary[tableMold.Name];

//                    foreach (var foreignKey in tableMold.ForeignKeys)
//                    {
//                        var referencedTableNode = dictionary.GetValueOrDefault(foreignKey.ReferencedTableName);
//                        if (referencedTableNode == null)
//                        {
//                            throw new TauDbException(
//                                $"Table '{foreignKey.ReferencedTableName}' not found."); // todo: standard exception 'object not found' to avoid copy/paste
//                        }

//                        tableNode.DrawEdgeTo(referencedTableNode);
//                    }
//                }

//                var algorithm = new GraphSlicingAlgorithm<TableMold>(graph);
//                var slices = algorithm.Slice();

//                if (!independentFirst.Value)
//                {
//                    slices = slices.Reverse().ToArray();
//                }

//                var arrangedTableNames = slices
//                    .SelectMany(x => x.Nodes.OrderBy(y => y.Value.Name))
//                    .Select(x => x.Value.Name)
//                    .ToList();

//                return arrangedTableNames;
//            }

//            return tableNames;
        }

        public static IList<ForeignKeyMold> GetTableForeignKeys(
            this SQLiteConnection connection,
            string tableName,
            bool loadColumns)
        {
            throw new NotImplementedException();

//            if (connection == null)
//            {
//                throw new ArgumentNullException(nameof(connection));
//            }

//            if (schemaName == null)
//            {
//                throw new ArgumentNullException(nameof(schemaName));
//            }

//            if (tableName == null)
//            {
//                throw new ArgumentNullException(nameof(tableName));
//            }

//            using var command = connection.CreateCommand();
//            command.CommandText = @"
//SELECT
//    TC.constraint_name ConstraintName,
//    RC.unique_constraint_name UniqueConstraintName,
//    TC2.table_name ReferencedTableName
//FROM
//    information_schema.table_constraints TC
//INNER JOIN
//    information_schema.referential_constraints RC
//ON
//    TC.constraint_name = RC.constraint_name
//INNER JOIN
//    information_schema.table_constraints TC2
//ON
//    TC2.constraint_name = RC.unique_constraint_name AND TC2.constraint_type = 'PRIMARY KEY'
//WHERE
//    TC.table_name = @p_tableName AND
//    TC.constraint_type = 'FOREIGN KEY' AND

//    TC.constraint_schema = @p_schemaName AND
//    TC.table_schema = @p_schemaName AND

//    RC.constraint_schema = @p_schemaName AND
//    RC.unique_constraint_schema = @p_schemaName AND

//    TC2.constraint_schema = @p_schemaName AND
//    TC2.table_schema = @p_schemaName
//";

//            command.Parameters.AddWithValue("@p_schemaName", schemaName);
//            command.Parameters.AddWithValue("@p_tableName", tableName);

//            var foreignKeyMolds = DbTools
//                .GetCommandRows(command)
//                .Select(x => new ForeignKeyMold
//                {
//                    Name = x.ConstraintName,
//                    ReferencedTableName = x.ReferencedTableName,
//                })
//                .ToList();

//            if (loadColumns)
//            {
//                command.CommandText = @"
//SELECT
//    CU.constraint_name  ConstraintName,
//    CU.column_name      ColumnName,
//    CU2.column_name     ReferencedColumnName
//FROM
//    information_schema.key_column_usage CU
//INNER JOIN
//    information_schema.referential_constraints RC
//ON
//    CU.constraint_name = RC.constraint_name
//INNER JOIN
//    information_schema.key_column_usage CU2
//ON
//    RC.unique_constraint_name = CU2.constraint_name AND
//    CU.ordinal_position = CU2.ordinal_position
//WHERE
//    CU.constraint_name = @p_fkName AND
//    CU.constraint_schema = @p_schemaName AND
//    CU.table_schema = @p_schemaName AND

//    CU2.CONSTRAINT_SCHEMA = @p_schemaName AND
//    CU2.TABLE_SCHEMA = @p_schemaName
//ORDER BY
//    CU.ordinal_position
//";

//                command.Parameters.Clear();

//                var fkParam = command.Parameters.Add("p_fkName", SqlDbType.NVarChar, 100);
//                var schemaParam = command.Parameters.Add("p_schemaName", SqlDbType.NVarChar, 100);
//                schemaParam.Value = schemaName;

//                command.Prepare();

//                foreach (var fk in foreignKeyMolds)
//                {
//                    fkParam.Value = fk.Name;

//                    var rows = DbTools.GetCommandRows(command);

//                    fk.ColumnNames = rows
//                        .Select(x => (string)x.ColumnName)
//                        .ToList();

//                    fk.ReferencedColumnNames = rows
//                        .Select(x => (string)x.ReferencedColumnName)
//                        .ToList();
//                }
//            }

//            return foreignKeyMolds;
        }


        public static long GetLastIdentity(this SQLiteConnection connection)
        {
            throw new NotImplementedException();
        }

        public static void DropTable(this SQLiteConnection connection, string tableName)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            using var command = connection.CreateCommand();
            command.CommandText = $"DROP TABLE [{tableName}]";
            command.ExecuteNonQuery();
        }

        public static IReadOnlyList<TableMold> GetTableMolds(
            this SQLiteConnection connection,
            bool? independentFirst)
        {
            throw new NotImplementedException();

            //var tableNames = GetTableNames(connection, schemaName, independentFirst);
            //var inspector = new SQLiteInspectorLab(connection);

            //return tableNames
            //    .Select(x => inspector.Factory.CreateTableInspector(connection, schemaName, x).GetTable())
            //    .ToList();
        }
    }
}
