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
            this.SchemaExplorer = new NpgsqlSchemaExplorer(this.NpgsqlConnection);
        }
        protected IDbSchemaExplorer SchemaExplorer { get; }

        protected NpgsqlConnection NpgsqlConnection => (NpgsqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactoryLab.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer2(IDbConnection connection) =>
            new NpgsqlSchemaExplorer(this.NpgsqlConnection);
    }
}
