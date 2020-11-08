using System.Data.SQLite;
using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteSerializerLab : DbSerializerBase
    {
        public SQLiteSerializerLab(SQLiteConnection connection)
            : base(connection, null)
        {
        }

        public override IDbUtilityFactory Factory => SQLiteUtilityFactoryLab.Instance;
    }
}
