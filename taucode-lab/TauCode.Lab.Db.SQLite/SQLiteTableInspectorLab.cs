using System.Data;
using System.Data.SQLite;
using TauCode.Db;
using TauCode.Db.Schema;

// todo: regions
namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteTableInspectorLab : DbTableInspectorBase
    {
        public SQLiteTableInspectorLab(SQLiteConnection connection, string tableName)
            : base(connection, null, tableName)
        {
        }

        public override IDbUtilityFactory Factory => SQLiteUtilityFactoryLab.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) =>
            new SQLiteSchemaExplorer((SQLiteConnection) this.Connection);
    }
}
