using MySql.Data.MySqlClient;
using System.Data;
using TauCode.Db;
using TauCode.Db.Schema;

namespace TauCode.Lab.Db.MySql
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
