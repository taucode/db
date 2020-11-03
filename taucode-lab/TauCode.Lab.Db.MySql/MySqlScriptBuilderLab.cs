using TauCode.Db;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlScriptBuilderLab : DbScriptBuilderBase
    {
        public MySqlScriptBuilderLab(string schemaName) : base(schemaName)
        {
        }

        public override IDbUtilityFactory Factory => MySqlUtilityFactoryLab.Instance;
    }
}
