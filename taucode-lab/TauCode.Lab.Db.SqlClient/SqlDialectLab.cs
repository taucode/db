using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient
{
    [DbDialect(
        typeof(SqlDialectLab),
        "reserved-words.txt",
        "[],\"\"")]
    public class SqlDialectLab : DbDialectBase
    {
        #region Static

        public static readonly SqlDialectLab Instance = new SqlDialectLab();

        #endregion

        #region Constructor

        private SqlDialectLab()
            : base(DbProviderNames.SQLServer)
        {
        }

        #endregion

        #region Overridden

        public override IDbUtilityFactory Factory => SqlUtilityFactoryLab.Instance;
        
        public override string UnicodeTextLiteralPrefix => "N";

        #endregion
    }
}
