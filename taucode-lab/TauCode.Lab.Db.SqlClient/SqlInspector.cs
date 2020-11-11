using Microsoft.Data.SqlClient;
using System.Data;
using TauCode.Db;
using TauCode.Db.Schema;

namespace TauCode.Lab.Db.SqlClient
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
