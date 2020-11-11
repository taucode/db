using Npgsql;
using System.Data;
using TauCode.Db;
using TauCode.Db.Schema;

namespace TauCode.Lab.Db.Npgsql
{
    public class NpgsqlTableInspectorLab : DbTableInspectorBase
    {
        public NpgsqlTableInspectorLab(NpgsqlConnection connection, string schemaName, string tableName)
            : base(
                connection,
                schemaName ?? NpgsqlToolsLab.DefaultSchemaName,
                tableName)
        {
        }

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactoryLab.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer2(IDbConnection connection) =>
            new NpgsqlSchemaExplorer((NpgsqlConnection) this.Connection);
    }
}
