using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public abstract class TableInspectorBase : UtilityBase, ITableInspector
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

        protected TableInspectorBase(
            IDbConnection connection,
            string tableName)
            : base(connection, true, false)
        {
            this.TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        }

        #endregion

        #region Polymorph

        protected abstract List<ColumnInfo> GetColumnInfos();

        protected abstract ColumnMold ColumnInfoToColumnMold(ColumnInfo columnInfo);

        protected abstract Dictionary<string, ColumnIdentityMold> GetIdentities();

        #endregion

        #region ITableInspector Members

        public string TableName { get; }

        public virtual IReadOnlyList<ColumnMold> GetColumns()
        {
            var columnInfos = this.GetColumnInfos();
            var columns = columnInfos
                .Select(this.ColumnInfoToColumnMold)
                .ToList();

            var identities = this.GetIdentities();

            foreach (var identityColumnName in identities.Keys)
            {
                var column = columns.Single(x =>
                    string.Equals(x.Name, identityColumnName, StringComparison.InvariantCultureIgnoreCase));

                column.Identity = identities[identityColumnName];
            }

            return columns;
        }

        public abstract PrimaryKeyMold GetPrimaryKey();

        public abstract IReadOnlyList<ForeignKeyMold> GetForeignKeys();

        public abstract IReadOnlyList<IndexMold> GetIndexes();

        public virtual TableMold GetTable()
        {
            var primaryKey = this.GetPrimaryKey();
            var columns = this.GetColumns();
            var foreignKeys = this.GetForeignKeys();
            var indexes = this.GetIndexes();

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
