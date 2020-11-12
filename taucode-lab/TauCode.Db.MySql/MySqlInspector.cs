using MySql.Data.MySqlClient;
using System.Data;
using TauCode.Db.MySql.Schema;
using TauCode.Db.Schema;

namespace TauCode.Db.MySql
{
    public class MySqlInspector : DbInspectorBase
    {
        public MySqlInspector(MySqlConnection connection)
            : base(connection, connection?.Database)
        {
            MySqlTools.CheckConnectionArgument(connection);
        }

        protected MySqlConnection MySqlConnection => (MySqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => MySqlUtilityFactory.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) =>
            new MySqlSchemaExplorer(this.MySqlConnection);
    }
}
