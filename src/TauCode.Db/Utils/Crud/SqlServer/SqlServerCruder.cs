using System.Data;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Building.SqlServer;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.SqlServer;

namespace TauCode.Db.Utils.Crud.SqlServer
{
    public class SqlServerCruder : CruderBase
    {
        public SqlServerCruder(IDbConnection connection)
            : base(connection)
        {
        }

        protected override string ExpectedDbConnectionTypeFullName => "System.Data.SqlClient.SqlConnection";

        protected override IScriptBuilder CreateScriptBuilder() => new SqlServerScriptBuilder();

        protected override IDbInspector CreateDbInspector() => new SqlServerInspector(this.GetSafeConnection());
    }
}
