using System.Data;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Building.MySql;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.MySql;

namespace TauCode.Db.Utils.Crud.MySql
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
