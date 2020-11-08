using System;
using System.Collections.Generic;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteInspectorLab : DbInspectorBase
    {
        public SQLiteInspectorLab(IDbConnection connection, string schemaName) : base(connection, schemaName)
        {
        }

        public override IDbUtilityFactory Factory => throw new NotImplementedException();
        protected override IReadOnlyList<string> GetTableNamesImpl(string schemaName)
        {
            throw new NotImplementedException();
        }

        protected override HashSet<string> GetSystemSchemata()
        {
            throw new NotImplementedException();
        }

        protected override bool NeedCheckSchemaExistence => throw new NotImplementedException();
        protected override bool SchemaExists(string schemaName)
        {
            throw new NotImplementedException();
        }
    }
}
