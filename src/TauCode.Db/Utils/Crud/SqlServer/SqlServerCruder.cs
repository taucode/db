using System.Data;
using TauCode.Db.Utils.Building.SqlServer;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.SqlServer;

namespace TauCode.Db.Utils.Crud.SqlServer
{
    public class SqlServerCruder : SqlServerCruderBase
    {
        public SqlServerCruder()
            : base(new SqlServerScriptBuilder())
        {
        }

        protected override ITableInspector GetTableInspectorImpl(IDbConnection connection, string tableName)
        {
            var dbInspector = new SqlServerInspector(connection);
            return dbInspector.GetTableInspector(tableName);
        }
    }
}
