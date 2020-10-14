using System.Collections.Generic;
using System.Data;
using TauCode.Db;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlTableInspectorLab : DbTableInspectorBase
    {
        public SqlTableInspectorLab(IDbConnection connection, string schemaName, string tableName) : base(connection, schemaName, tableName)
        {
        }

        public override IDbUtilityFactory Factory { get; }
        protected override List<ColumnInfo> GetColumnInfos()
        {
            throw new System.NotImplementedException();
        }

        protected override ColumnMold ColumnInfoToColumnMold(ColumnInfo columnInfo)
        {
            throw new System.NotImplementedException();
        }

        protected override Dictionary<string, ColumnIdentityMold> GetIdentities()
        {
            throw new System.NotImplementedException();
        }

        public override PrimaryKeyMold GetPrimaryKey()
        {
            throw new System.NotImplementedException();
        }

        public override IReadOnlyList<ForeignKeyMold> GetForeignKeys()
        {
            throw new System.NotImplementedException();
        }

        public override IReadOnlyList<IndexMold> GetIndexes()
        {
            throw new System.NotImplementedException();
        }
    }
}
