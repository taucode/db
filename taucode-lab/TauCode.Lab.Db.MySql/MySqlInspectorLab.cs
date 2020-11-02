using System;
using System.Collections.Generic;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlInspectorLab : DbInspectorBase
    {
        public MySqlInspectorLab(IDbConnection connection, string schemaName) : base(connection, schemaName)
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
