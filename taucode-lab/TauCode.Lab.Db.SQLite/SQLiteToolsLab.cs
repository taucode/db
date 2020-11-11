using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Extensions;

// todo clean
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

        //public static IReadOnlyList<string> GetTableNames(
        //    this SQLiteConnection connection,
        //    bool? independentFirst) => 
        //    GetTableMolds(connection, independentFirst, false)
        //    .Select(x => x.Name)
        //    .ToList();

        //public static TableMold ParseTableCreationSql(string sql)
        //{
        //    if (sql == null)
        //    {
        //        throw new ArgumentNullException(sql);
        //    }

        //    var parser = SQLiteParser.Instance;
        //    var objs = parser.Parse(sql);

        //    if (objs.Length != 1 || !(objs.Single() is TableMold))
        //    {
        //        throw new ArgumentException($"Could not build table definition from script:{Environment.NewLine}{sql}", nameof(sql));
        //    }

        //    return objs.Single() as TableMold;
        //}

        //public static long GetLastIdentity(this SQLiteConnection connection)
        //{
        //    using var command = connection.CreateCommand();
        //    command.CommandText = "SELECT last_insert_rowid()";
        //    return (long)command.ExecuteScalar();
        //}

        //public static void DropTable(this SQLiteConnection connection, string tableName)
        //{
        //    if (connection == null)
        //    {
        //        throw new ArgumentNullException(nameof(connection));
        //    }

        //    if (tableName == null)
        //    {
        //        throw new ArgumentNullException(nameof(tableName));
        //    }

        //    using var command = connection.CreateCommand();
        //    command.CommandText = $"DROP TABLE [{tableName}]";
        //    command.ExecuteNonQuery();
        //}

//        public static IReadOnlyList<TableMold> GetTableMolds(
//            this SQLiteConnection connection,
//            bool? independentFirst,
//            bool includeIndexes)
//        {
//            if (connection == null)
//            {
//                throw new ArgumentNullException(nameof(connection));
//            }

//            using var command = connection.CreateCommand();
//            command.CommandText =
//                @"
//SELECT
//    T.name  Name,
//    T.sql   Sql
//FROM
//    sqlite_master T
//WHERE
//    T.type = 'table' AND
//    T.name NOT LIKE 'sqlite_%'
//ORDER BY
//    T.name
//";

//            var rows = command.GetCommandRows();

//            if (rows.Count == 0)
//            {
//                return new List<TableMold>();
//            }

//            if (independentFirst.HasValue)
//            {
//                var tableMolds = rows
//                    .Select(x => (TableMold)ParseTableCreationSql(x.Sql))
//                    .ToList();

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

//                var arrangedTables = slices
//                    .SelectMany(x => x.Nodes.OrderBy(y => y.Value.Name))
//                    .Select(x => x.Value)
//                    .ToList();

//                if (includeIndexes)
//                {
//                    foreach (var arrangedTable in arrangedTables)
//                    {
//                        var indexes = GetTableIndexes(connection, arrangedTable.Name);
//                        arrangedTable.Indexes = indexes;
//                    }
//                }

//                return arrangedTables;
//            }
//            else
//            {
//                var tableMolds = rows
//                    .Select(x => (TableMold)ParseTableCreationSql(x.Sql))
//                    .ToList();

//                if (includeIndexes)
//                {
//                    foreach (var tableMold in tableMolds)
//                    {
//                        var indexes = GetTableIndexes(connection, tableMold.Name);
//                        tableMold.Indexes = indexes;
//                    }
//                }

//                return tableMolds;
//            }
//        }

//        public static bool TableExists(this SQLiteConnection connection, string tableName)
//            => connection.GetTableNames(null).Contains(tableName); // todo optimize

//        public static TableMold GetTableMold(this SQLiteConnection connection, string tableName)
//        {
//            if (connection == null)
//            {
//                throw new ArgumentNullException(nameof(connection));
//            }

//            if (tableName == null)
//            {
//                throw new ArgumentNullException(nameof(tableName));
//            }

//            using var command = connection.CreateCommand();
//            command.CommandText =
//                @"
//SELECT
//    T.name  Name,
//    T.sql   Sql
//FROM
//    sqlite_master T
//WHERE
//    T.type = 'table' AND
//    T.name NOT LIKE 'sqlite_%' AND
//    T.name = @p_tableName
//ORDER BY
//    T.name
//";

//            command.Parameters.AddWithValue("p_tableName", tableName);

//            var rows = command.GetCommandRows();

//            if (rows.Count == 0)
//            {
//                throw DbTools.CreateTableDoesNotExistException(null, tableName);
//            }

//            var row = rows.Single();
//            var tableMold = ParseTableCreationSql((string)row.Sql).ResolveExplicitPrimaryKey();
//            return tableMold;
//        }

//        public static IList<IndexMold> GetTableIndexes(this SQLiteConnection connection, string tableName)
//        {
//            if (connection == null)
//            {
//                throw new ArgumentNullException(nameof(connection));
//            }

//            if (tableName == null)
//            {
//                throw new ArgumentNullException(nameof(tableName));
//            }

//            using var command = connection.CreateCommand();
//            command.CommandText =
//                @"
//SELECT
//    T.name    Name,
//    T.sql     Sql
//FROM
//    sqlite_master T
//WHERE
//    T.type = 'index' AND
//    T.tbl_name = @p_tableName AND
//    T.name NOT LIKE 'sqlite_%'
//ORDER BY
//    T.name
//";

//            command.Parameters.AddWithValue("p_tableName", tableName);
//            //command.Parameters.AddWithValue("p_antiPattern", "sqlite_autoindex_%");

//            var parser = SQLiteParser.Instance;

//            var indexes = command
//                .GetCommandRows()
//                .Select(x => (IndexMold)parser.Parse((string)x.Sql).Single())
//                .ToList();

//            return indexes;
//        }

        internal static TableMold ResolveExplicitPrimaryKey(this TableMold tableMold)
        {
            var pkColumn = tableMold.Columns.SingleOrDefault(x =>
                ((Dictionary<string, string>)x.Properties).GetValueOrDefault("#is_explicit_primary_key") == "true");

            if (pkColumn != null)
            {
                tableMold.PrimaryKey = new PrimaryKeyMold
                {
                    Name = $"PK_{tableMold.Name}",
                    Columns = new List<string>
                    {
                        pkColumn.Name
                    },
                };
            }

            return tableMold;
        }
    }
}
