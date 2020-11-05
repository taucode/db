using MySql.Data.MySqlClient;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlSerializerLab : DbSerializerBase
    {
        public MySqlSerializerLab(MySqlConnection connection, string schemaName) : base(connection, schemaName)
        {
        }

        public override IDbUtilityFactory Factory => MySqlUtilityFactoryLab.Instance;
    }
}
