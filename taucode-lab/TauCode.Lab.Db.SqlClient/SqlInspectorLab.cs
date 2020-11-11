using Microsoft.Data.SqlClient;
using System.Data;
using TauCode.Db;
using TauCode.Db.Schema;

// todo clean & regions.
namespace TauCode.Lab.Db.SqlClient
{
    public class SqlInspectorLab : DbInspectorBase
    {
        public SqlInspectorLab(SqlConnection connection, string schemaName)
            : base(connection, schemaName ?? SqlToolsLab.DefaultSchemaName)
        {
        }

        protected SqlConnection SqlConnection => (SqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => SqlUtilityFactoryLab.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) =>
            new SqlSchemaExplorer(this.SqlConnection);
    }
}
