using System.Data;
using MySql.Data.MySqlClient;
using TauCode.Db.Schema;

namespace TauCode.Db.MySql
{
    public class MySqlInspector : DbInspectorBase
    {
        public MySqlInspector(MySqlConnection connection)
            : base(connection, connection?.Database)
        {
        }

        protected MySqlConnection MySqlConnection => (MySqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => MySqlUtilityFactory.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) =>
            new MySqlSchemaExplorer(this.MySqlConnection);
    }
}
