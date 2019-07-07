namespace TauCode.Db.Utils.Dialects.SQLite
{
    [Dialect(
        "sql-sqlite-reserved-words.txt",
        "sql-sqlite-data-type-names.txt",
        "\"\",[],``")]
    public class SQLiteDialect : DialectBase
    {
        #region Static

        public static readonly SQLiteDialect Instance = new SQLiteDialect();

        #endregion

        #region Constructor

        private SQLiteDialect()
            : base("SQLite")
        {
        }

        #endregion

        #region Overridden

        public override bool CanDecorateTypeIdentifier => false;

        #endregion
    }
}
