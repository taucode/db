namespace TauCode.Db.SqlServer
{
    public class SqlServerScriptBuilderLab : ScriptBuilderLabBase
    {
        public override IUtilityFactory Factory => SqlServerUtilityFactory.Instance;
    }
}
