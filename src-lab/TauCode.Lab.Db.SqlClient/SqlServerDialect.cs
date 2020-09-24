using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient
{
    [Dialect(
        typeof(SqlServerDialect),
        "reserved-words.txt",
        "data-type-names.txt",
        "[],\"\"")]
    public class SqlServerDialect : DialectBase
    {
        #region Static

        public static readonly SqlServerDialect Instance = new SqlServerDialect();

        #endregion

        #region Constructor

        private SqlServerDialect()
            : base("SQL Server")
        {
        }

        #endregion

        #region Overridden

        public override IUtilityFactory Factory => SqlServerUtilityFactory.Instance;
        
        public override string UnicodeTextLiteralPrefix => "N";

        #endregion
    }
}
