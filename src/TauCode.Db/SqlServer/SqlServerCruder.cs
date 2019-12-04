using System.Data;

namespace TauCode.Db.SqlServer
{
    public class SqlServerCruder : CruderBase
    {
        public SqlServerCruder(IDbConnection connection)
            : base(connection)
        {
        }

        //protected override string ExpectedDbConnectionTypeFullName => "System.Data.SqlClient.SqlConnection";

        //protected override IScriptBuilder CreateScriptBuilder() => new SqlServerScriptBuilder();

        //protected override IDbInspector CreateDbInspector() => new SqlServerInspector(this.GetSafeConnection());
        public override IUtilityFactory Factory => SqlServerUtilityFactory.Instance;
    }
}
