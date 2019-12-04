using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Algorithms.Graphs;
using TauCode.Db.Model;

namespace TauCode.Db
{
    // todo: nice regions
    public abstract class DbInspectorBase : UtilityBase, IDbInspector
    {
        protected DbInspectorBase(IDbConnection connection)
            : base(connection, true, false)
        {
        }

        protected abstract IReadOnlyList<string> GetTableNamesImpl();

        public IReadOnlyList<string> GetTableNames(bool? independentFirst = null)
        {
            var tableNames = this.GetTableNamesImpl();
            if (independentFirst.HasValue)
            {
                tableNames = this.GetSortedTableNames(tableNames, independentFirst.Value)
                    .Select(x => x.Name)
                    .ToList();
            }

            return tableNames;
        }

        protected virtual IReadOnlyList<TableMold> GetSortedTableNames(IReadOnlyList<string> tableNames, bool independentFirst)
        {
            var tableMolds = tableNames
                .Select(x => this.Factory.CreateTableInspector(this.Connection, x))
                .Select(x => x.GetTable())
                .ToList();

            var graph = new Graph<TableMold>();

            foreach (var tableMold in tableMolds)
            {
                graph.AddNode(tableMold);
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

            return list;
        }

        //public abstract ITableInspector GetTableInspector(string tableName);
    }
}
