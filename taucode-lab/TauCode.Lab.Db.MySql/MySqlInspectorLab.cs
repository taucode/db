using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;
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

        protected override IDbSchemaExplorer CreateSchemaExplorer2(IDbConnection connection) =>
            new MySqlSchemaExplorer(this.MySqlConnection);

        public override IReadOnlyList<string> GetTableNames()
            => this.SchemaExplorer.GetTableNames(this.SchemaName).ToList(); // todo
    }
}
