using Microsoft.Data.SqlClient;
using System.Data;
using TauCode.Db;
using TauCode.Db.Schema;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlTableInspectorLab : DbTableInspectorBase
    {
        public SqlTableInspectorLab(SqlConnection connection, string schemaName, string tableName)
            : base(
                connection,
                schemaName ?? SqlToolsLab.DefaultSchemaName,
                tableName)
        {
        }

        public override IDbUtilityFactory Factory => SqlUtilityFactoryLab.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer2(IDbConnection connection) =>
            new SqlSchemaExplorer((SqlConnection) this.Connection);
    }
}
