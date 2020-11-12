using System.Data;
using Microsoft.Data.SqlClient;
using TauCode.Db.Schema;
using TauCode.Db.SqlClient.Schema;

namespace TauCode.Db.SqlClient
{
    public class SqlInspector : DbInspectorBase
    {
        public SqlInspector(SqlConnection connection, string schemaName)
            : base(connection, schemaName ?? SqlTools.DefaultSchemaName)
        {
        }

        protected SqlConnection SqlConnection => (SqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => SqlUtilityFactory.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) =>
            new SqlSchemaExplorer(this.SqlConnection);
    }
}
