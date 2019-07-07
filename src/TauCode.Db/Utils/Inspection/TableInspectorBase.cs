using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Db.Utils.Crud;
using TauCode.Db.Utils.Dialects;

namespace TauCode.Db.Utils.Inspection
{
    public abstract class TableInspectorBase : ITableInspector
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

        #region Fields

        private ICruder _cruder;

        #endregion

        #region Constructor

        protected TableInspectorBase(
            IDialect dialect,
            IDbConnection connection,
            string tableName)
        {
            this.Dialect = dialect ?? throw new ArgumentNullException(nameof(dialect));
            this.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        }

        #endregion

        #region Polymorph

        protected abstract List<ColumnInfo> GetColumnInfos();

        protected abstract ColumnMold ColumnInfoToColumnMold(ColumnInfo columnInfo);
        
        protected abstract Dictionary<string, ColumnIdentityMold> GetIdentities();

        protected abstract ICruder CreateCruder();

        #endregion

        #region Protected

        protected IDbConnection Connection { get; }

        protected ICruder Cruder => _cruder ?? (_cruder = this.CreateCruder());

        #endregion

        #region ITableInspector Members

        public IDialect Dialect { get; }

        public string TableName { get; }

        public virtual List<ColumnMold> GetColumnMolds()
        {
            var columnInfos = this.GetColumnInfos();
            var columns = columnInfos
                .Select(this.ColumnInfoToColumnMold)
                .ToList();

            var identities = this.GetIdentities();

            foreach (var identityColumnName in identities.Keys)
            {
                var column = columns.Single(x => string.Equals(x.Name, identityColumnName, StringComparison.InvariantCultureIgnoreCase));
                column.Identity = identities[identityColumnName];
            }

            return columns;
        }

        public abstract PrimaryKeyMold GetPrimaryKeyMold();

        public abstract List<ForeignKeyMold> GetForeignKeyMolds();

        public abstract List<IndexMold> GetIndexMolds();

        public virtual TableMold GetTableMold()
        {
            var primaryKey = this.GetPrimaryKeyMold();
            var columns = this.GetColumnMolds();
            var foreignKeys = this.GetForeignKeyMolds();
            var indexes = this.GetIndexMolds();

            var table = new TableMold
            {
                Name = this.TableName,
                PrimaryKey = primaryKey,
                Columns = columns,
                ForeignKeys = foreignKeys,
                Indexes = indexes,
            };

            return table;
        }

        #endregion
    }
}
