using System.Data;
using System.Data.SQLite;
using TauCode.Db;
using TauCode.Db.Schema;

namespace TauCode.Lab.Db.SQLite
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
