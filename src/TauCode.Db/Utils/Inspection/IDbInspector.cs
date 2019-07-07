using System.Data;
using TauCode.Db.Utils.Building;

namespace TauCode.Db.Utils.Inspection
{
    public interface IDbInspector
    {
        IDbConnection Connection { get; }

        IScriptBuilder CreateScriptBuilder();

        string[] GetTableNames();

        ITableInspector GetTableInspector(string tableName);
    }
}
