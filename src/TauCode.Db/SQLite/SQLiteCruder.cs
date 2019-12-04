using System.Data;

namespace TauCode.Db.SQLite
{
    public class SQLiteCruder : CruderBase
    {
        public SQLiteCruder(IDbConnection connection)
            : base(connection)
        {
        }

        //protected override string ExpectedDbConnectionTypeFullName => "System.Data.SQLite.SQLiteConnection";

        //protected override IScriptBuilder CreateScriptBuilder() => new SQLiteScriptBuilder();

        //protected override IDbInspector CreateDbInspector() => new SQLiteInspector(this.GetSafeConnection());
        protected override IUtilityFactory GetFactoryImpl()
        {
            throw new System.NotImplementedException();
        }
    }
}
