using System.Data;
using Npgsql;

namespace TauCode.Db.Npgsql
{
    public class NpgsqlInspector : DbInspectorBase
    {
        public NpgsqlInspector(NpgsqlConnection connection, string schemaName)
            : base(connection, schemaName ?? NpgsqlTools.DefaultSchemaName)
        {
        }

        protected NpgsqlConnection NpgsqlConnection => (NpgsqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactory.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) =>
            new NpgsqlSchemaExplorer(this.NpgsqlConnection);
    }
}
