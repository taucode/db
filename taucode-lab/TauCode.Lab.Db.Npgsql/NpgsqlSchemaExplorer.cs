using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Db.Schema;

namespace TauCode.Lab.Db.Npgsql
{
    public class NpgsqlSchemaExplorer : DbSchemaExplorerBase
    {
        public NpgsqlSchemaExplorer(NpgsqlConnection connection)
            : base(connection, "\"\"")
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
            return NpgsqlToolsLab.SystemSchemata.ToList(); // todo!
        }
    }
}
