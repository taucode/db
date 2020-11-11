using System.Data.SQLite;
using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteSerializer : DbSerializerBase
    {
        public SQLiteSerializer(SQLiteConnection connection)
            : base(connection, null)
        {
        }

        public override IDbUtilityFactory Factory => SQLiteUtilityFactory.Instance;
    }
}
