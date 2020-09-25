using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlSerializer : DbSerializerBase
    {
        public MySqlSerializer(IDbConnection connection, string schema)
            : base(connection, schema)

        {
        }
        public override IDbUtilityFactory Factory => MySqlUtilityFactory.Instance;
    }
}
