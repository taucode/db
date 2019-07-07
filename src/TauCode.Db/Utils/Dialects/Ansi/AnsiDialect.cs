namespace TauCode.Db.Utils.Dialects.Ansi
{
    [Dialect(
        "sql-ansi-reserved-words.txt",
        "sql-ansi-data-type-names.txt",
        "\"\"")]
    public class AnsiDialect : DialectBase
    {
        #region Static

        public static readonly AnsiDialect Instance = new AnsiDialect();

        #endregion

        #region Constructor

        private AnsiDialect()
            : base("ANSI SQL")
        {
        }

        #endregion
    }
}
