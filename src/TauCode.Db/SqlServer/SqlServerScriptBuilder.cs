namespace TauCode.Db.SqlServer
{
    public class SqlServerScriptBuilder : ScriptBuilderBase
    {
        public override IUtilityFactory Factory => SqlServerUtilityFactory.Instance;
    }
}
