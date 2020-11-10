using System;
using System.Collections.Generic;
using System.Data.SQLite;
using TauCode.Db.Model;
using TauCode.Db.Schema;

// todo: move to 'Schema' sub-namespace, here & anywhere
namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteSchemaExplorer : DbSchemaExplorerBase
    {
        public SQLiteSchemaExplorer(SQLiteConnection connection)
            : base(connection, "[]")
        {
        }

        protected override ColumnMold ColumnInfoToColumn(ColumnInfo2 columnInfo)
        {
            throw new NotImplementedException();
        }

        protected override IReadOnlyList<IndexMold> GetTableIndexesImpl(string schemaName, string tableName)
        {
            throw new NotImplementedException();
        }

        protected override void ResolveIdentities(string schemaName, string tableName, IList<ColumnInfo2> columnInfos)
        {
            throw new NotImplementedException();
        }

        public override IReadOnlyList<string> GetSystemSchemata()
        {
            throw new NotImplementedException();
        }
    }
}
