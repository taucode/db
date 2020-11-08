using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Model;

// todo: regions
namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteTableInspectorLab : DbTableInspectorBase
    {
        public SQLiteTableInspectorLab(SQLiteConnection connection, string tableName)
            : base(connection, null, tableName)
        {
        }

        public SQLiteConnection SQLiteConnection => (SQLiteConnection)this.Connection;

        public override IDbUtilityFactory Factory => SQLiteUtilityFactoryLab.Instance;

        protected override List<ColumnInfo> GetColumnInfos()
        {
            throw new NotSupportedException(); // should not be called, actually.
        }

        protected override ColumnMold ColumnInfoToColumnMold(ColumnInfo columnInfo)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, ColumnIdentityMold> GetIdentities()
        {
            throw new NotImplementedException();
        }

        protected override bool NeedCheckSchemaExistence => false;

        protected override bool SchemaExists(string schemaName) => throw new NotSupportedException();

        protected override bool TableExists(string tableName) => this.SQLiteConnection.TableExists(tableName);

        public override IReadOnlyList<ColumnMold> GetColumnsImpl()
        {
            throw new NotSupportedException($"Use method '{nameof(GetTable)}'."); // todo: copy-paste
        }

        public override TableMold GetTable()
        {
            var tableMold = this.SQLiteConnection.GetTableMold(this.TableName);
            var indexes = this.SQLiteConnection.GetTableIndexes(this.TableName);
            tableMold.Indexes = indexes;
            return tableMold;
        }

        protected override PrimaryKeyMold GetPrimaryKeyImpl()
        {
            throw new NotSupportedException($"Use method '{nameof(GetTable)}'.");
        }

        protected override IReadOnlyList<ForeignKeyMold> GetForeignKeysImpl()
        {
            throw new NotSupportedException($"Use method '{nameof(GetTable)}'."); // todo: copy-pasted
        }

        protected override IReadOnlyList<IndexMold> GetIndexesImpl() =>
            this.SQLiteConnection.GetTableIndexes(this.TableName).ToList();
    }
}
