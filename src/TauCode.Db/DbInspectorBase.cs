using System.Collections.Generic;
using System.Data;
using TauCode.Db.Schema;

namespace TauCode.Db
{
    // todo get rid of '2'
    public abstract class DbInspectorBase : DbUtilityBase, IDbInspector
    {
        #region Fields

        private IDbSchemaExplorer _schemaExplorer;

        #endregion

        #region Constructor

        protected DbInspectorBase(IDbConnection connection, string schemaName)
            : base(connection, true, false)
        {
            this.SchemaName = schemaName;
        }

        #endregion

        #region Protected

        protected IDbSchemaExplorer SchemaExplorer2 => _schemaExplorer ??= this.CreateSchemaExplorer2(this.Connection);

        protected abstract IDbSchemaExplorer CreateSchemaExplorer2(IDbConnection connection);

        #endregion

        #region IDbInspector Members

        public string SchemaName { get; }

        public virtual IReadOnlyList<string> GetSchemaNames() => this.SchemaExplorer2.GetSchemata();

        public virtual IReadOnlyList<string> GetTableNames() => this.SchemaExplorer2.GetTableNames(this.SchemaName);

        #endregion
    }
}
