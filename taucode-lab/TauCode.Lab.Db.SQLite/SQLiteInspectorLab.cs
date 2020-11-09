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

        public SQLiteConnection SQLiteConnection => (SQLiteConnection)this.Connection;

        public override IDbUtilityFactory Factory => SQLiteUtilityFactoryLab.Instance;

        public override IReadOnlyList<string> GetSchemaNames() => new string[] { };

        protected override IReadOnlyList<string> GetTableNamesImpl(string schemaName)
            => this.SQLiteConnection.GetTableNames(null);

        protected override HashSet<string> GetSystemSchemata() => new HashSet<string>();

        protected override bool NeedCheckSchemaExistence => false;

        protected override bool SchemaExists(string schemaName)
        {
            throw new NotSupportedException();
        }
    }
}
