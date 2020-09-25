using TauCode.Db;

namespace TauCode.Lab.Db.Npgsql
{
    [DbDialect(
        typeof(NpgsqlDialect),
        "reserved-words.txt",
        //"data-type-names.txt",
        "\"\"")]
    public class NpgsqlDialect : DbDialectBase
    {
        #region Static

        public static readonly NpgsqlDialect Instance = new NpgsqlDialect();

        #endregion

        #region Constructor

        private NpgsqlDialect()
            : base(DbProviderNames.SQLServer)
        {
        }

        #endregion

        #region Overridden

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactory.Instance;
        
        public override string UnicodeTextLiteralPrefix => "N";

        #endregion
    }
}
