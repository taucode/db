using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Algorithms.Graphs;
using TauCode.Db.Model;
using TauCode.Db.Utils.Building;

namespace TauCode.Db.Utils.Inspection
{
    public static class DbInspectorExtensions
    {
        public static TableMold[] GetOrderedTableMolds(this IDbInspector dbInspector, bool independentFirst)
        {
            if (dbInspector == null)
            {
                throw new ArgumentNullException(nameof(dbInspector));
            }

            var graph = new Graph<TableMold>();

            var tables = dbInspector
                .GetTableNames()
                .Select(x => dbInspector.GetTableInspector(x).GetTableMold())
                .ToArray();

            foreach (var table in tables)
            {
                graph.AddNode(table);
            }

            foreach (var node in graph.Nodes)
            {
                var table = node.Value;
                foreach (var foreignKey in table.ForeignKeys)
                {
                    var referencedNode = graph.Nodes.Single(x => x.Value.Name == foreignKey.ReferencedTableName);
                    node.DrawEdgeTo(referencedNode);
                }
            }

            var algorithm = new GraphSlicingAlgorithm<TableMold>(graph);
            var slices = algorithm.Slice();
            if (!independentFirst)
            {
                slices = slices.Reverse().ToArray();
            }

            var list = new List<TableMold>();

            foreach (var slice in slices)
            {
                var sliceTables = slice.Nodes
                    .Select(x => x.Value)
                    .OrderBy(x => x.Name);

                list.AddRange(sliceTables);
            }

            return list.ToArray();
        }

        public static void ClearDb(this IDbInspector dbInspector)
        {
            var scriptBuilder = dbInspector.CreateScriptBuilder();
            var script = scriptBuilder.BuildClearDbSql(dbInspector.Connection);
            var statements = ScriptBuilderBase.SplitSqlByComments(script);
            using (var command = dbInspector.Connection.CreateCommand())
            {
                foreach (var statement in statements)
                {
                    command.CommandText = statement;
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void PurgeDb(this IDbInspector dbInspector)
        {
            dbInspector.ClearDb();
            var tables = dbInspector.GetOrderedTableMolds(false);
            var scriptBuilder = dbInspector.CreateScriptBuilder();

            using (var command = dbInspector.Connection.CreateCommand())
            {
                foreach (var table in tables)
                {
                    var sql = scriptBuilder.BuildDropTableSql(table.Name);
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
        }

        public static void ExecuteSql(this IDbInspector dbInspector, string sql)
        {
            using (var command = dbInspector.Connection.CreateCommand())
            {
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }
    }
}
