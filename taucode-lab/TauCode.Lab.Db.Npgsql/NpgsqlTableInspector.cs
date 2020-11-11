using Npgsql;
using System.Data;
using TauCode.Db;
using TauCode.Db.Schema;

namespace TauCode.Lab.Db.Npgsql
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
