using System.Collections.Generic;
using System.Data;

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
                tableNames = this.GetSortedTableNames(independentFirst.Value);
            }

            return tableNames;
        }

        protected virtual IReadOnlyList<string> GetSortedTableNames(bool independentFirst)
        {
            throw new System.NotImplementedException();
        }

        public abstract ITableInspector GetTableInspector(string tableName);
    }
}
