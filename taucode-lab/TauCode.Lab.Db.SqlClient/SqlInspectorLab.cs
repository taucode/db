using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlInspectorLab : DbInspectorBase
    {
        public SqlInspectorLab(IDbConnection connection, string schemaName)
            : base(connection, schemaName ?? SqlToolsLab.DefaultSchemaName)
        {
        }

        protected SqlConnection SqlConnection => (SqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => SqlUtilityFactoryLab.Instance;

        protected override IReadOnlyList<string> GetTableNamesImpl(string schemaName) =>
            this.SqlConnection.GetTableNames(this.SchemaName, null);

        protected override HashSet<string> GetSystemSchemata() => SqlToolsLab.SystemSchemata;
    }
}
