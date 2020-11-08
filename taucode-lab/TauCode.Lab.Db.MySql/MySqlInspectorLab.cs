using MySql.Data.MySqlClient;
using System.Collections.Generic;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlInspectorLab : DbInspectorBase
    {
        public MySqlInspectorLab(MySqlConnection connection)
            : base(connection, connection.GetSchemaName())
        {
        }

        protected MySqlConnection MySqlConnection => (MySqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => MySqlUtilityFactoryLab.Instance;

        protected override IReadOnlyList<string> GetTableNamesImpl(string schemaName) =>
            this.MySqlConnection.GetTableNames(this.SchemaName, null);

        protected override HashSet<string> GetSystemSchemata() => MySqlToolsLab.SystemSchemata;

        protected override bool NeedCheckSchemaExistence => true;

        protected override bool SchemaExists(string schemaName) => this.MySqlConnection.SchemaExists(schemaName);
    }
}
