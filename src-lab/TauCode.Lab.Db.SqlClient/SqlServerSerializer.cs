using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlServerSerializer : DbSerializerBase
    {
        public SqlServerSerializer(IDbConnection connection)
            : base(connection)

        {
        }
        public override IDbUtilityFactory Factory => SqlServerUtilityFactory.Instance;
    }
}
