using System.Data;

namespace TauCode.Db.SQLite
{
    public class SQLiteSerializer : DbSerializerBase
    {
        public SQLiteSerializer(IDbConnection connection)
            : base(connection)
        {
        }

        public override IUtilityFactory Factory => SQLiteUtilityFactory.Instance;
    }
}
