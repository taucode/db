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

        protected IDbSchemaExplorer SchemaExplorer2 => _schemaExplorer ??= this.CreateSchemaExplorer2(this.Connection);

        protected abstract IDbSchemaExplorer CreateSchemaExplorer2(IDbConnection connection);

        #endregion

        #region ITableInspector Members

        public string SchemaName { get; }

        public string TableName { get; }

        public virtual IReadOnlyList<ColumnMold> GetColumns() =>             
            this.SchemaExplorer2
            .GetTableColumns(this.SchemaName, this.TableName, true);

        public virtual PrimaryKeyMold GetPrimaryKey() =>
            this.SchemaExplorer2.GetTablePrimaryKey(this.SchemaName, this.TableName, true);

        public virtual IReadOnlyList<ForeignKeyMold> GetForeignKeys()
            => this.SchemaExplorer2.GetTableForeignKeys(this.SchemaName, this.TableName, true, true);

        public virtual IReadOnlyList<IndexMold> GetIndexes()
            => this.SchemaExplorer2.GetTableIndexes(this.SchemaName, this.TableName, true);
        
        public virtual TableMold GetTable() => this.SchemaExplorer2.GetTable(
            this.SchemaName,
            this.TableName,
            true,
            true,
            true,
            true);

        #endregion
    }
}
