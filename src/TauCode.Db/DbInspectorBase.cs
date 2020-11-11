using System.Collections.Generic;
using System.Data;
using TauCode.Db.Schema;

namespace TauCode.Db
{
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

        protected IDbSchemaExplorer SchemaExplorer => _schemaExplorer ??= this.CreateSchemaExplorer(this.Connection);

        protected abstract IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection);

        #endregion

        #region IDbInspector Members

        public string SchemaName { get; }

        public virtual IReadOnlyList<string> GetSchemaNames() => this.SchemaExplorer.GetSchemata();

        public virtual IReadOnlyList<string> GetTableNames() => this.SchemaExplorer.GetTableNames(this.SchemaName);

        #endregion
    }
}
