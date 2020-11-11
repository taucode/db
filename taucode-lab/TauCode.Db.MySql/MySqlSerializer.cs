using MySql.Data.MySqlClient;

namespace TauCode.Db.MySql
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
