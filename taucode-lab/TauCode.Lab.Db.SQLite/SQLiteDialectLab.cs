using System;
using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
{
    // todo regions
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

        public override IDbUtilityFactory Factory => throw new NotImplementedException();
    }
}
