using System;
using System.Collections.Generic;
using System.Data;
using TauCode.Db.Model;
using TauCode.Db.Schema;

namespace TauCode.Db
{
    public abstract class DbTableInspectorBase : DbUtilityBase, IDbTableInspector
    {
        #region Fields

        private IDbSchemaExplorer _schemaExplorer;

        #endregion

        #region Constructor

        protected DbTableInspectorBase(
            IDbConnection connection,
            string schemaName,
            string tableName)
            : base(connection, true, false)
        {
            this.SchemaName = schemaName;
            this.TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        }

        #endregion

        #region Protected

        protected IDbSchemaExplorer SchemaExplorer => _schemaExplorer ??= this.CreateSchemaExplorer(this.Connection);

        protected abstract IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection);

        #endregion

        #region ITableInspector Members

        public string SchemaName { get; }

        public string TableName { get; }

        public virtual IReadOnlyList<ColumnMold> GetColumns() =>             
            this.SchemaExplorer
            .GetTableColumns(this.SchemaName, this.TableName, true);

        public virtual PrimaryKeyMold GetPrimaryKey() =>
            this.SchemaExplorer.GetTablePrimaryKey(this.SchemaName, this.TableName, true);

        public virtual IReadOnlyList<ForeignKeyMold> GetForeignKeys()
            => this.SchemaExplorer.GetTableForeignKeys(this.SchemaName, this.TableName, true, true);

        public virtual IReadOnlyList<IndexMold> GetIndexes()
            => this.SchemaExplorer.GetTableIndexes(this.SchemaName, this.TableName, true);
        
        public virtual TableMold GetTable() => this.SchemaExplorer.GetTable(
            this.SchemaName,
            this.TableName,
            true,
            true,
            true,
            true);

        #endregion
    }
}
