using System.Data;
using System.Data.SQLite;
using TauCode.Db.Schema;
using TauCode.Db.SQLite.Schema;

namespace TauCode.Db.SQLite
{
    public class SQLiteInspector : DbInspectorBase
    {
        public SQLiteInspector(SQLiteConnection connection)
            : base(connection, null)
        {
        }

        public override IDbUtilityFactory Factory => SQLiteUtilityFactory.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) =>
            new SQLiteSchemaExplorer((SQLiteConnection)connection);
    }
}
