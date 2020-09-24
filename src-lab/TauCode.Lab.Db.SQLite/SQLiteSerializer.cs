using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteSerializer : DbSerializerBase
    {
        public SQLiteSerializer(IDbConnection connection)
            : base(connection)
        {
        }

        public override IDbUtilityFactory Factory => SQLiteUtilityFactory.Instance;
    }
}
