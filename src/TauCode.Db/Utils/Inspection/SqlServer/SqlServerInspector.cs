using System.Data;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Building.SqlServer;
using TauCode.Db.Utils.Crud;
using TauCode.Db.Utils.Crud.SqlServer;

namespace TauCode.Db.Utils.Inspection.SqlServer
{
    public class SqlServerInspector : SqlServerInspectorBase
    {
        #region Constructor

        public SqlServerInspector(IDbConnection connection)
            : base(connection)
        {
        }

        #endregion

        #region Overridden

        protected override string TableTypeForTable => "BASE TABLE";

        protected override SqlServerTableInspectorBase CreateTableInspectorImpl(string realTableName)
        {
            return new SqlServerTableInspector(this.Connection, realTableName);
        }

        protected override ICruder CreateCruder()
        {
            return new SqlServerCruder();
        }

        public override IScriptBuilder CreateScriptBuilder()
        {
            return new SqlServerScriptBuilder
            {
                CurrentOpeningIdentifierDelimiter = '[',
            };
        }

        #endregion
    }
}
