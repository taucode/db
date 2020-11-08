using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteInspectorLab : DbInspectorBase
    {
        public SQLiteInspectorLab(SQLiteConnection connection)
            : base(connection, null)
        {
        }

        public override IDbUtilityFactory Factory => SQLiteUtilityFactoryLab.Instance;

        protected override IReadOnlyList<string> GetTableNamesImpl(string schemaName)
        {
            throw new NotImplementedException();
        }

        protected override HashSet<string> GetSystemSchemata() => new HashSet<string>();

        protected override bool NeedCheckSchemaExistence => false;

        protected override bool SchemaExists(string schemaName)
        {
            throw new NotSupportedException();
        }
    }
}
