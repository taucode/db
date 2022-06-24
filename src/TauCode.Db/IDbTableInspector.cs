using System.Collections.Generic;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public interface IDbTableInspector : IDbUtility
    {
        string SchemaName { get; }

        string TableName { get; }

        IReadOnlyList<ColumnMold> GetColumns();

        PrimaryKeyMold GetPrimaryKey();

        IReadOnlyList<ForeignKeyMold> GetForeignKeys();

        IReadOnlyList<IndexMold> GetIndexes();

        TableMold GetTable();
    }
}
