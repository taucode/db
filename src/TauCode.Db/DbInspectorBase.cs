using System.Collections.Generic;
using System.Data;

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

            return tableNames;
        }

        #endregion
    }
}
