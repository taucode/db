using MySql.Data.MySqlClient;
using System.Data;

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
    }
}
