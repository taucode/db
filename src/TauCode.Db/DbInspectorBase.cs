using System.Collections.Generic;
using System.Data;

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

        protected virtual IDbSchemaExplorer CreateSchemaExplorer() =>
            this.Factory.CreateSchemaExplorer(this.Connection);

        protected IDbSchemaExplorer SchemaExplorer => _schemaExplorer ??= this.CreateSchemaExplorer();

        #endregion

        #region IDbInspector Members

        public string SchemaName { get; }

        public virtual IReadOnlyList<string> GetSchemaNames() => this.SchemaExplorer.GetSchemaNames();

        public virtual IReadOnlyList<string> GetTableNames() => this.SchemaExplorer.GetTableNames(this.SchemaName);

        #endregion
    }
}
