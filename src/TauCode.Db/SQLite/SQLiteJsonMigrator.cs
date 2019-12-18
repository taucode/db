using System;
using System.Data;

namespace TauCode.Db.SQLite
{
    public class SQLiteJsonMigrator : JsonMigratorBase
    {
        public SQLiteJsonMigrator(IDbConnection connection, Func<string> metadataJsonGetter, Func<string> dataJsonGetter)
            : base(connection, metadataJsonGetter, dataJsonGetter)
        {
        }

        public override IUtilityFactory Factory => SQLiteUtilityFactory.Instance;
    }
}
