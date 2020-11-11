using System.Data;
using MySql.Data.MySqlClient;
using TauCode.Db.Schema;

namespace TauCode.Db.MySql
{
    public class MySqlTableInspector : DbTableInspectorBase
    {
        public MySqlTableInspector(MySqlConnection connection, string tableName)
            : base(connection, connection?.Database, tableName)
        {
        }

        public override IDbUtilityFactory Factory => MySqlUtilityFactory.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) =>
            new MySqlSchemaExplorer((MySqlConnection) this.Connection);
    }
}
