using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteScriptBuilderLab : DbScriptBuilderBase
    {
        public SQLiteScriptBuilderLab()
            : base(null)
        {
        }

        public override IDbUtilityFactory Factory => SQLiteUtilityFactoryLab.Instance;
    }
}
