using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using TauCode.Db;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlTableInspectorLab : DbTableInspectorBase
    {
        public MySqlTableInspectorLab(MySqlConnection connection, string schemaName, string tableName)
            : base(connection, schemaName, tableName)
        {
        }

        public override IDbUtilityFactory Factory => throw new NotImplementedException();
        protected override List<ColumnInfo> GetColumnInfos()
        {
            throw new NotImplementedException();
        }

        protected override ColumnMold ColumnInfoToColumnMold(ColumnInfo columnInfo)
        {
            throw new NotImplementedException();
        }

        protected override Dictionary<string, ColumnIdentityMold> GetIdentities()
        {
            throw new NotImplementedException();
        }

        protected override bool NeedCheckSchemaExistence => throw new NotImplementedException();
        protected override bool SchemaExists(string schemaName)
        {
            throw new NotImplementedException();
        }

        protected override bool TableExists(string tableName)
        {
            throw new NotImplementedException();
        }

        protected override PrimaryKeyMold GetPrimaryKeyImpl()
        {
            throw new NotImplementedException();
        }

        protected override IReadOnlyList<ForeignKeyMold> GetForeignKeysImpl()
        {
            throw new NotImplementedException();
        }

        protected override IReadOnlyList<IndexMold> GetIndexesImpl()
        {
            throw new NotImplementedException();
        }
    }
}
