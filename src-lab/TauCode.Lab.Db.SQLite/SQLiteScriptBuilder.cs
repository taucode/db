using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteScriptBuilder : DbScriptBuilderBase
    {
        #region Constructor

        public SQLiteScriptBuilder()
        :base(null)
        {
            // todo: schema must be null, ut
        }

        #endregion

        #region Overridden

        public override IDbUtilityFactory Factory => SQLiteUtilityFactory.Instance;

        #endregion
    }
}
