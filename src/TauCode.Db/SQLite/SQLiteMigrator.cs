using System;

namespace TauCode.Db.SQLite
{
    public class SQLiteMigrator : DbMigratorBase
    {
        public SQLiteMigrator(
            SQLiteSerializer serializer,
            Func<string> metadataJsonGetter,
            Func<string> dataJsonGetter)
            : base(serializer, metadataJsonGetter, dataJsonGetter)
        {
        }

        public override IUtilityFactory Factory => SQLiteUtilityFactory.Instance;
    }
}
