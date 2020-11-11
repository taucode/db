using MySql.Data.MySqlClient;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlSerializerLab : DbSerializerBase
    {
        public MySqlSerializerLab(MySqlConnection connection)
            : base(connection, connection?.Database)
        {
        }

        public override IDbUtilityFactory Factory => MySqlUtilityFactoryLab.Instance;
    }
}
