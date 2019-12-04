using System.Data;

namespace TauCode.Db.SqlServer
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
