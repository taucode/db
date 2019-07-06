using System.Collections.Generic;
using TauCode.Db.Model;
using TauCode.Db.Utils.Dialects;

namespace TauCode.Db.Utils.Inspection
{
    public interface ITableInspector
    {
        IDialect Dialect { get; }

        string TableName { get; }

        List<ColumnMold> GetColumnMolds();

        PrimaryKeyMold GetPrimaryKeyMold();

        List<ForeignKeyMold> GetForeignKeyMolds();

        List<IndexMold> GetIndexMolds();

        TableMold GetTableMold();
    }
}