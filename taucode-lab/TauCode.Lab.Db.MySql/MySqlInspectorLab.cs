using MySql.Data.MySqlClient;
using System.Data;
using TauCode.Db;
using TauCode.Db.Schema;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlInspectorLab : DbInspectorBase
    {
        public MySqlInspectorLab(MySqlConnection connection)
            : base(connection, connection?.Database)
        {
        }

        protected MySqlConnection MySqlConnection => (MySqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => MySqlUtilityFactoryLab.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) =>
            new MySqlSchemaExplorer(this.MySqlConnection);
    }
}
