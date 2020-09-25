using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Algorithms.Graphs;
using TauCode.Db.Model;

// todo clean
namespace TauCode.Db
{
    public abstract class DbInspectorBase : DbUtilityBase, IDbInspector
    {
        #region Constructor

        protected DbInspectorBase(IDbConnection connection, string schema)
            : base(connection, true, false)
        {
            this.Schema = schema;
        }

        #endregion

        #region Abstract & Virtual

        protected abstract IReadOnlyList<string> GetTableNamesImpl(string schema);

        protected virtual IReadOnlyList<TableMold> GetSortedTableNames(IReadOnlyList<string> tableNames, bool independentFirst)
        {
            var tableMolds = tableNames
                .Select(x => this.Factory.CreateTableInspector(this.Connection, this.Schema, x))
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

        #endregion

        #region IDbInspector Members

        public string Schema { get; }

        public IReadOnlyList<string> GetSchemata()
        {
            throw new System.NotImplementedException();
        }

        public IReadOnlyList<string> GetTableNames()
        {
            var tableNames = this.GetTableNamesImpl(this.Schema);
            //if (independentFirst.HasValue)
            //{
            //    tableNames = this.GetSortedTableNames(tableNames, independentFirst.Value)
            //        .Select(x => x.Name)
            //        .ToList();
            //}

            return tableNames;
        }

        #endregion
    }
}
