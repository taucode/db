using MySql.Data.MySqlClient;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlSerializer : DbSerializerBase
    {
        public MySqlSerializer(MySqlConnection connection)
            : base(connection, connection?.Database)
        {
        }

        public override IDbUtilityFactory Factory => MySqlUtilityFactory.Instance;
    }
}
