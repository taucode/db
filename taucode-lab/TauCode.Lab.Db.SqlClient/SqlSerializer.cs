using Microsoft.Data.SqlClient;
using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlSerializer : DbSerializerBase
    {
        public SqlSerializer(SqlConnection connection, string schema)
            : base(connection, schema ?? SqlTools.DefaultSchemaName)

        {
        }

        public override IDbUtilityFactory Factory => SqlUtilityFactory.Instance;
    }
}
