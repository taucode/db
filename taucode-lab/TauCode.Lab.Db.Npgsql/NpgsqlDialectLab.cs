using TauCode.Db;

namespace TauCode.Lab.Db.Npgsql
{
    [DbDialect(
        typeof(NpgsqlDialectLab),
        "reserved-words.txt",
        "\"\"")]
    public class NpgsqlDialectLab : DbDialectBase
    {
        #region Static

        public static readonly NpgsqlDialectLab Instance = new NpgsqlDialectLab();

        #endregion

        private NpgsqlDialectLab()
            : base(DbProviderNames.PostgreSQL)
        {
        }

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactoryLab.Instance;

        public override bool CanDecorateTypeIdentifier => false;
    }
}
