using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteScriptBuilder : DbScriptBuilderBase
    {
        #region Constructor

        public SQLiteScriptBuilder()
        {
        }

        #endregion

        #region Overridden

        public override IDbUtilityFactory Factory => SQLiteUtilityFactory.Instance;

        #endregion
    }
}
