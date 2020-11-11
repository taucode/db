using Npgsql;
using System.Data;
using TauCode.Db;
using TauCode.Db.Schema;

namespace TauCode.Lab.Db.Npgsql
{
    public class NpgsqlInspectorLab : DbInspectorBase
    {
        public NpgsqlInspectorLab(NpgsqlConnection connection, string schemaName)
            : base(connection, schemaName ?? NpgsqlToolsLab.DefaultSchemaName)
        {
        }

        protected NpgsqlConnection NpgsqlConnection => (NpgsqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactoryLab.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) =>
            new NpgsqlSchemaExplorer(this.NpgsqlConnection);
    }
}
