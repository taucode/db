using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using TauCode.Db;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlTableInspectorLab : DbTableInspectorBase
    {
        public MySqlTableInspectorLab(MySqlConnection connection, string tableName)
            : base(connection, connection.GetSchemaName(), tableName)
        {
        }

        protected MySqlConnection MySqlConnection => (MySqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => MySqlUtilityFactoryLab.Instance;

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

        protected override bool NeedCheckSchemaExistence => true;

        protected override bool SchemaExists(string schemaName) => this.MySqlConnection.SchemaExists(schemaName);

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
