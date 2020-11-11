using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
{
    [DbDialect(
        typeof(SQLiteDialectLab),
        "reserved-words.txt",
        "[],\"\"")]
    public class SQLiteDialectLab : DbDialectBase
    {
        public static readonly SQLiteDialectLab Instance = new SQLiteDialectLab();

        private SQLiteDialectLab()
            : base(DbProviderNames.SQLite)
        {
        }

        public override bool CanDecorateTypeIdentifier => false;

        public override IDbUtilityFactory Factory => SQLiteUtilityFactoryLab.Instance;
    }
}
