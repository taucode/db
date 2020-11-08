using System;
using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteScriptBuilderLab : DbScriptBuilderBase
    {
        public SQLiteScriptBuilderLab(string schemaName) : base(schemaName)
        {
        }

        public override IDbUtilityFactory Factory => throw new NotImplementedException();
    }
}
