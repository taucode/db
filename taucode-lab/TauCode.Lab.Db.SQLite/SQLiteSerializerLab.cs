using System;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteSerializerLab : DbSerializerBase
    {
        public SQLiteSerializerLab(IDbConnection connection, string schemaName) : base(connection, schemaName)
        {
        }

        public override IDbUtilityFactory Factory => throw new NotImplementedException();
    }
}
