using System.Data;
using TauCode.Db.Utils.Dialects.SqlServer;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.SqlServer;

namespace TauCode.Db.Utils.Building.SqlServer
{
    public class SqlServerScriptBuilder : SqlServerScriptBuilderBase
    {
        #region Constructor

        public SqlServerScriptBuilder()
            : base(SqlServerDialect.Instance)
        {
        }

        #endregion

        #region Overridden

        protected override IDbInspector CreateDbInspector(IDbConnection connection)
        {
            return new SqlServerInspector(connection);
        }

        #endregion
    }
}
