using System;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
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
