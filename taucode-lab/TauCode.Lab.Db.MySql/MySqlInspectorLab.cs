using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Schema;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlInspectorLab : DbInspectorBase
    {
        public MySqlInspectorLab(MySqlConnection connection)
            : base(connection, connection?.Database)
        {
            this.SchemaExplorer = new MySqlSchemaExplorer(this.MySqlConnection);
        }

        protected IDbSchemaExplorer SchemaExplorer { get; }

        protected MySqlConnection MySqlConnection => (MySqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => MySqlUtilityFactoryLab.Instance;

        protected override IReadOnlyList<string> GetTableNamesImpl(string schemaName) =>
            //this.MySqlConnection.GetTableNames(this.SchemaName, null);
            throw new NotImplementedException();

        public override IReadOnlyList<string> GetTableNames()
            => this.SchemaExplorer.GetTableNames(this.SchemaName).ToList(); // todo

        protected override HashSet<string> GetSystemSchemata() => MySqlToolsLab.SystemSchemata;

        protected override bool NeedCheckSchemaExistence => throw new NotImplementedException();

        protected override bool SchemaExists(string schemaName)
            //=> this.MySqlConnection.SchemaExists(schemaName);
            => throw new NotImplementedException();
    }
}
