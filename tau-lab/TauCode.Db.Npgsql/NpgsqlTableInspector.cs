using System.Data;
using Npgsql;

namespace TauCode.Db.Npgsql
{
    public class NpgsqlTableInspector : DbTableInspectorBase
    {
        public NpgsqlTableInspector(NpgsqlConnection connection, string schemaName, string tableName)
            : base(
                connection,
                schemaName ?? NpgsqlTools.DefaultSchemaName,
                tableName)
        {
        }

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactory.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) =>
            new NpgsqlSchemaExplorer((NpgsqlConnection) this.Connection);
    }
}
