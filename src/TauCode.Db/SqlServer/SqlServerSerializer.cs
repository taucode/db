using System.Data;

namespace TauCode.Db.SqlServer
{
    public class SqlServerSerializer : DbSerializerBase
    {
        public SqlServerSerializer(IDbConnection connection)
            : base(connection)

        {
        }
        public override IUtilityFactory Factory => SqlServerUtilityFactory.Instance;
    }
}
