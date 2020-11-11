using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TauCode.Db;
using TauCode.Db.Model;
using TauCode.Db.Schema;

// todo: regions
namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteTableInspectorLab : DbTableInspectorBase
    {
        public SQLiteTableInspectorLab(SQLiteConnection connection, string tableName)
            : base(connection, null, tableName)
        {
            this.SchemaExplorer = new SQLiteSchemaExplorer(this.SQLiteConnection);
        }

        public SQLiteConnection SQLiteConnection => (SQLiteConnection)this.Connection;

        protected IDbSchemaExplorer SchemaExplorer { get; }

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

        public override IReadOnlyList<ColumnMold> GetColumns()
        {
            return this
                .SchemaExplorer
                .GetTableColumns(this.SchemaName, this.TableName, true);
        }

        public override IReadOnlyList<ForeignKeyMold> GetForeignKeys()
        {
            return this
                .SchemaExplorer
                .GetTableForeignKeys(this.SchemaName, this.TableName, true, true);
        }

        public override PrimaryKeyMold GetPrimaryKey()
        {
            return this.SchemaExplorer
                .GetTablePrimaryKey(this.SchemaName, this.TableName, true);
        }

        protected override bool TableExists(string tableName)
            //=> this.SQLiteConnection.TableExists(tableName);
            => throw new NotImplementedException();

        public override IReadOnlyList<IndexMold> GetIndexes()
        {
            return this.SchemaExplorer
                .GetTableIndexes(this.SchemaName, this.TableName, true);
        }

        public override IReadOnlyList<ColumnMold> GetColumnsImpl()
        {
            throw new NotSupportedException($"Use method '{nameof(GetTable)}'."); // todo: copy-paste
        }

        public override TableMold GetTable() => this.SchemaExplorer.GetTable(
            this.SchemaName,
            this.TableName,
            true,
            true,
            true,
            true);

        protected override PrimaryKeyMold GetPrimaryKeyImpl()
        {
            throw new NotSupportedException($"Use method '{nameof(GetTable)}'.");
        }

        protected override IReadOnlyList<ForeignKeyMold> GetForeignKeysImpl()
        {
            throw new NotSupportedException($"Use method '{nameof(GetTable)}'."); // todo: copy-pasted
        }

        protected override IReadOnlyList<IndexMold> GetIndexesImpl() =>
            //this.SQLiteConnection.GetTableIndexes(this.TableName).ToList();
            throw new NotImplementedException();
    }
}
