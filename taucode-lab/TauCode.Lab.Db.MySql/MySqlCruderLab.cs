using MySql.Data.MySqlClient;
using System;
using System.Data;
using TauCode.Db;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlCruderLab : DbCruderBase
    {
        public MySqlCruderLab(MySqlConnection connection, string schemaName) : base(connection, schemaName)
        {
        }

        public override IDbUtilityFactory Factory => MySqlUtilityFactoryLab.Instance;

        protected override IDbValueConverter CreateDbValueConverter(ColumnMold column)
        {
            throw new NotImplementedException();
        }

        protected override IDbDataParameter CreateParameter(string tableName, ColumnMold column)
        {
            throw new NotImplementedException();
        }
    }
}
