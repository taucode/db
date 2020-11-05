using TauCode.Db;

namespace TauCode.Lab.Db.MySql
{
    [DbDialect(
        typeof(MySqlDialectLab),
        "reserved-words.txt",
        "``")]

    public class MySqlDialectLab : DbDialectBase
    {
        public static readonly MySqlDialectLab Instance = new MySqlDialectLab();

        private MySqlDialectLab()
            : base(DbProviderNames.MySQL)
        {
        }

        public override IDbUtilityFactory Factory => MySqlUtilityFactoryLab.Instance;

        public override bool CanDecorateTypeIdentifier => false;
    }
}
