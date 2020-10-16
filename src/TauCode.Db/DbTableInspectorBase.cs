using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db.Model;

namespace TauCode.Db
{
    // todo clean
    public abstract class DbTableInspectorBase : DbUtilityBase, IDbTableInspector
    {
        #region Nested

        protected class ColumnInfo
        {
            public string Name { get; set; }
            public string TypeName { get; set; }
            public bool IsNullable { get; set; }
            public int? Size { get; set; }
            public int? Precision { get; set; }
            public int? Scale { get; set; }
            public Dictionary<string, string> AdditionalProperties { get; set; } = new Dictionary<string, string>();
        }

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

        #region Private

        private void CheckSchemaIfNeeded()
        {
            if (this.NeedCheckSchemaExistence)
            {
                if (!this.SchemaExists(this.SchemaName))
                {
                    throw DbTools.CreateSchemaDoesNotExistException(this.SchemaName);
                }
            }
        }

        private void CheckTable()
        {
            var exists = this.TableExists(this.TableName);
            if (!exists)
            {
                throw DbTools.CreateTableDoesNotExistException(this.SchemaName, this.TableName);
            }
        }

        #endregion

        #region Polymorph

        protected abstract List<ColumnInfo> GetColumnInfos();

        protected abstract ColumnMold ColumnInfoToColumnMold(ColumnInfo columnInfo);

        protected abstract Dictionary<string, ColumnIdentityMold> GetIdentities();

        protected abstract bool NeedCheckSchemaExistence { get; }

        protected abstract bool SchemaExists(string schemaName);

        protected abstract bool TableExists(string tableName);

        public virtual IReadOnlyList<ColumnMold> GetColumnsImpl()
        {
            var columnInfos = this.GetColumnInfos();
            var columns = columnInfos
                .Select(this.ColumnInfoToColumnMold)
                .ToList();

            var identities = this.GetIdentities();

            foreach (var identityColumnName in identities.Keys)
            {
                var column = columns.SingleOrDefault(x => x.Name == identityColumnName);
                if (column == null)
                {
                    // should not happen.
                    throw DbTools.CreateInternalErrorException();
                }

                column.Identity = identities[identityColumnName];
            }

            return columns;
        }

        protected abstract PrimaryKeyMold GetPrimaryKeyImpl();

        protected abstract IReadOnlyList<ForeignKeyMold> GetForeignKeysImpl();

        protected abstract IReadOnlyList<IndexMold> GetIndexesImpl();

        #endregion

        #region ITableInspector Members

        public string SchemaName { get; }

        public string TableName { get; }

        public IReadOnlyList<ColumnMold> GetColumns()
        {
            this.CheckSchemaIfNeeded();
            this.CheckTable();

            var columns = this.GetColumnsImpl();
            return columns;
        }

        public PrimaryKeyMold GetPrimaryKey()
        {
            this.CheckSchemaIfNeeded();
            this.CheckTable();

            var primaryKey = this.GetPrimaryKeyImpl();
            return primaryKey;
        }

        public IReadOnlyList<ForeignKeyMold> GetForeignKeys()
        {
            this.CheckSchemaIfNeeded();
            this.CheckTable();

            var foreignKeys = this.GetForeignKeysImpl();
            return foreignKeys;
        }

        public IReadOnlyList<IndexMold> GetIndexes()
        {
            this.CheckSchemaIfNeeded();
            this.CheckTable();

            var indexes = this.GetIndexesImpl();
            return indexes;
        }

        public virtual TableMold GetTable()
        {
            this.CheckSchemaIfNeeded();
            this.CheckTable();

            var primaryKey = this.GetPrimaryKeyImpl();
            var columns = this.GetColumnsImpl();
            var foreignKeys = this.GetForeignKeysImpl();
            var indexes = this.GetIndexesImpl();

            var table = new TableMold
            {
                Name = this.TableName,
                PrimaryKey = primaryKey,
                Columns = columns.ToList(),
                ForeignKeys = foreignKeys.ToList(),
                Indexes = indexes.ToList(),
            };

            return table;
        }

        #endregion
    }
}
