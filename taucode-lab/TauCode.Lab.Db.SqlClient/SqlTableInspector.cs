using Microsoft.Data.SqlClient;
using System.Data;
using TauCode.Db;
using TauCode.Db.Schema;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlTableInspector : DbTableInspectorBase
    {
        public SqlTableInspector(SqlConnection connection, string schemaName, string tableName)
            : base(
                connection,
                schemaName ?? SqlTools.DefaultSchemaName,
                tableName)
        {
        }

        public override IDbUtilityFactory Factory => SqlUtilityFactory.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) =>
            new SqlSchemaExplorer((SqlConnection) this.Connection);
    }
}
