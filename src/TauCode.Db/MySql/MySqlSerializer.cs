using System.Data;

namespace TauCode.Db.MySql
{
    public class MySqlSerializer : DbSerializerBase
    {
        public MySqlSerializer(IDbConnection connection)
            : base(connection)
        {
        }

        public override IUtilityFactory Factory => MySqlUtilityFactory.Instance;
    }
}
