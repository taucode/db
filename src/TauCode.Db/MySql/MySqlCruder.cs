using System.Data;

namespace TauCode.Db.MySql
{
    public class MySqlCruder : CruderBase
    {
        public MySqlCruder(IDbConnection connection)
            : base(connection)
        {
        }

        protected override string ExpectedDbConnectionTypeFullName => "MySql.Data.MySqlClient.MySqlConnection";
        protected override IScriptBuilder CreateScriptBuilder() => new MySqlScriptBuilder();
        protected override IDbInspector CreateDbInspector() => new MySqlInspector(this.GetSafeConnection());
    }
}
