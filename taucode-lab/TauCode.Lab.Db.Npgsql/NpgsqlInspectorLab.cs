using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
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

        protected override IReadOnlyList<string> GetTableNamesImpl(string schemaName) =>
            //this.NpgsqlConnection.GetTableNames(this.SchemaName, null);
            throw new NotImplementedException();

        protected override HashSet<string> GetSystemSchemata() =>
            //NpgsqlToolsLab.SystemSchemata;
            this.SchemaExplorer.GetSystemSchemata().ToHashSet();

        public override IReadOnlyList<string> GetTableNames()
        {
            return this.SchemaExplorer.GetTableNames(this.SchemaName);
        }

        protected override bool NeedCheckSchemaExistence => throw new NotImplementedException();

        protected override bool SchemaExists(string schemaName) => throw new NotImplementedException();
    }
}
