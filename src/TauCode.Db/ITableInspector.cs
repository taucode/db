using System.Collections.Generic;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public interface ITableInspector : IUtility
    {
        //IDialect Dialect { get; }

        string TableName { get; }

        List<ColumnMold> GetColumns();

        PrimaryKeyMold GetPrimaryKey();

        List<ForeignKeyMold> GetForeignKeys();

        List<IndexMold> GetIndexes();

        TableMold GetTable();
    }
}
