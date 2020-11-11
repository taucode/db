using MySql.Data.MySqlClient;
using System.Data;
using TauCode.Db;
using TauCode.Db.Schema;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlTableInspectorLab : DbTableInspectorBase
    {
        public MySqlTableInspectorLab(MySqlConnection connection, string tableName)
            : base(connection, connection?.Database, tableName)
        {
        }

        public override IDbUtilityFactory Factory => MySqlUtilityFactoryLab.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) =>
            new MySqlSchemaExplorer((MySqlConnection) this.Connection);
    }
}
