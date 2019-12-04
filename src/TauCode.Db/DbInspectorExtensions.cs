using System;
using System.Data;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public static class DbInspectorExtensions
    {
        public static TableMold[] GetOrderedTableMolds(this IDbInspector dbInspector, bool independentFirst)
        {
            throw new NotImplementedException();
            //if (dbInspector == null)
            //{
            //    throw new ArgumentNullException(nameof(dbInspector));
            //}

            //var graph = new Graph<TableMold>();

            //var tables = dbInspector
            //    .GetTableNames()
            //    .Select(x => dbInspector.GetTableInspector(x).GetTableMold())
            //    .ToArray();

            //foreach (var table in tables)
            //{
            //    graph.AddNode(table);
            //}

            //foreach (var node in graph.Nodes)
            //{
            //    var table = node.Value;
            //    foreach (var foreignKey in table.ForeignKeys)
            //    {
            //        var referencedNode = graph.Nodes.Single(x => x.Value.Name == foreignKey.ReferencedTableName);
            //        node.DrawEdgeTo(referencedNode);
            //    }
            //}

            //var algorithm = new GraphSlicingAlgorithm<TableMold>(graph);
            //var slices = algorithm.Slice();
            //if (!independentFirst)
            //{
            //    slices = slices.Reverse().ToArray();
            //}

            //var list = new List<TableMold>();

            //foreach (var slice in slices)
            //{
            //    var sliceTables = slice.Nodes
            //        .Select(x => x.Value)
            //        .OrderBy(x => x.Name);

            //    list.AddRange(sliceTables);
            //}

            //return list.ToArray();
        }

        // todo: description (removes data from tables)
        public static void ClearData(this IDbInspector dbInspector)
        {
            throw new NotImplementedException();
            //var scriptBuilder = dbInspector.CreateScriptBuilder();
            //var script = scriptBuilder.BuildClearDbSql(dbInspector.Connection);
            //var statements = ScriptBuilderBase.SplitSqlByComments(script);
            //using (var command = dbInspector.Connection.CreateCommand())
            //{
            //    foreach (var statement in statements)
            //    {
            //        command.CommandText = statement;
            //        command.ExecuteNonQuery();
            //    }
            //}
        }

        // todo: description (removes tables and indexes)
        public static void PurgeDb(this IDbInspector dbInspector)
        {
            throw new NotImplementedException();
            //dbInspector.ClearData();
            //var tables = dbInspector.GetOrderedTableMolds(false);
            //var scriptBuilder = dbInspector.CreateScriptBuilder();

            //using (var command = dbInspector.Connection.CreateCommand())
            //{
            //    foreach (var table in tables)
            //    {
            //        var sql = scriptBuilder.BuildDropTableSql(table.Name);
            //        command.CommandText = sql;
            //        command.ExecuteNonQuery();
            //    }
            //}
        }

        public static void ExecuteSql(this IDbInspector dbInspector, string sql)
        {
            throw new NotImplementedException();
            //using (var command = dbInspector.Connection.CreateCommand())
            //{
            //    command.CommandText = sql;
            //    command.ExecuteNonQuery();
            //}
        }

        public static void ExecuteScript(this IDbConnection connection, string script)
        {
            var sqls = DbUtils.SplitScriptByComments(script);

            foreach (var sql in sqls)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
