using System.Data;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Building.SqlServer;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.SqlServer;

namespace TauCode.Db.Utils.Crud.SqlServer
{
    // todo clean up
    public class SqlServerCruder : /*SqlServerCruderBase*/ CruderBase
    {
        public SqlServerCruder(IDbConnection connection)
            : base(connection)
        {
        }

        //protected override ITableInspector GetTableInspectorImpl(IDbConnection connection, string tableName)
        //{
        //    var dbInspector = new SqlServerInspector(connection);
        //    return dbInspector.GetTableInspector(tableName);
        //}

        protected override string ExpectedDbConnectionTypeFullName => "System.Data.SqlClient.SqlConnection";

        protected override IScriptBuilder CreateScriptBuilder() => new SqlServerScriptBuilder();

        protected override IDbInspector CreateDbInspector() => new SqlServerInspector(this.GetSafeConnection());
    }
}
