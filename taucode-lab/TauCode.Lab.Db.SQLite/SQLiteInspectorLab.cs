using System.Data;
using System.Data.SQLite;
using TauCode.Db;
using TauCode.Db.Schema;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteInspectorLab : DbInspectorBase
    {
        public SQLiteInspectorLab(SQLiteConnection connection)
            : base(connection, null)
        {
        }

        public override IDbUtilityFactory Factory => SQLiteUtilityFactoryLab.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) =>
            new SQLiteSchemaExplorer((SQLiteConnection)connection);
    }
}
