using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlSerializerLab : DbSerializerBase
    {
        public SqlSerializerLab(IDbConnection connection, string schema)
            : base(connection, schema)

        {
        }

        public override IDbUtilityFactory Factory => SqlUtilityFactoryLab.Instance;
    }
}
