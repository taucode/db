using System.Collections.Generic;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public interface ITableInspector : IUtility
    {
        string TableName { get; }

        IReadOnlyList<ColumnMold> GetColumns();

        PrimaryKeyMold GetPrimaryKey();

        IReadOnlyList<ForeignKeyMold> GetForeignKeys();

        IReadOnlyList<IndexMold> GetIndexes();

        TableMold GetTable();
    }
}
