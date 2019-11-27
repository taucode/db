using System.Data;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Building.SQLite;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.SQLite;

namespace TauCode.Db.Utils.Crud.SQLite
{
    public class SQLiteCruder : CruderBase
    {
        public SQLiteCruder(IDbConnection connection)
            : base(connection)
        {
        }

        protected override string ExpectedDbConnectionTypeFullName => "System.Data.SQLite.SQLiteConnection";

        protected override IScriptBuilder CreateScriptBuilder() => new SQLiteScriptBuilder();

        protected override IDbInspector CreateDbInspector() => new SQLiteInspector(this.GetSafeConnection());
    }
}
