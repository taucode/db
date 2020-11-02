using System;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlSerializerLab : DbSerializerBase
    {
        public MySqlSerializerLab(IDbConnection connection, string schemaName) : base(connection, schemaName)
        {
        }

        public override IDbUtilityFactory Factory => throw new NotImplementedException();
    }
}
