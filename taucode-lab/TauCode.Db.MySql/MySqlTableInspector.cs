using MySql.Data.MySqlClient;
using System.Data;

namespace TauCode.Db.MySql
{
    public class MySqlTableInspector : DbTableInspectorBase
    {
        public MySqlTableInspector(MySqlConnection connection, string tableName)
            : base(connection, connection?.Database, tableName)
        {
            MySqlTools.CheckConnectionArgument(connection);
        }

        public override IDbUtilityFactory Factory => MySqlUtilityFactory.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) =>
            new MySqlSchemaExplorer((MySqlConnection) this.Connection);
    }
}
