namespace TauCode.Db.SQLite
{
    public class SQLiteScriptBuilder : ScriptBuilderBase
    {
        #region Constructor

        public SQLiteScriptBuilder()
        {
        }

        #endregion

        #region Overridden

        public override IUtilityFactory Factory => SQLiteUtilityFactory.Instance;

        #endregion
    }
}
