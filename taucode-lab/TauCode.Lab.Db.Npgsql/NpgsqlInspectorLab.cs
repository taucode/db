using Npgsql;
using System.Collections.Generic;
using TauCode.Db;

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

        protected override IReadOnlyList<string> GetTableNamesImpl(string schemaName) =>
            this.NpgsqlConnection.GetTableNames(this.SchemaName, null);

        protected override HashSet<string> GetSystemSchemata() => NpgsqlToolsLab.SystemSchemata;

        protected override bool NeedCheckSchemaExistence => true;
        protected override bool SchemaExists(string schemaName) => this.NpgsqlConnection.SchemaExists(schemaName);
    }
}
