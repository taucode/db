using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlSerializer : DbSerializerBase
    {
        public MySqlSerializer(IDbConnection connection)
            : base(connection, null)

        {
        }
        public override IDbUtilityFactory Factory => MySqlUtilityFactory.Instance;
    }
}
