using System;

namespace TauCode.Db.Ansi
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
            throw new NotImplementedException();
        }

        #endregion

        protected override IUtilityFactory GetFactoryImpl()
        {
            throw new NotImplementedException();
        }
    }
}
