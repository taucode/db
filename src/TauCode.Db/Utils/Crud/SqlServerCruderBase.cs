using TauCode.Db.Utils.Building;

namespace TauCode.Db.Utils.Crud
{
    public abstract class SqlServerCruderBase : CruderBase
    {
        protected SqlServerCruderBase(SqlServerScriptBuilderBase scriptBuilder)
            : base(scriptBuilder)
        {
        }
    }
}
