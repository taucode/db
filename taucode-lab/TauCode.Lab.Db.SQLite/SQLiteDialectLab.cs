using System;
using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteDialectLab : DbDialectBase
    {
        public SQLiteDialectLab(string name) : base(name)
        {
        }

        public override IDbUtilityFactory Factory => throw new NotImplementedException();
    }
}
