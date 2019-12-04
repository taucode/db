using System.Data;

namespace TauCode.Db
{
    public interface IDbInspector
    {
        IDbConnection Connection { get; }

        IScriptBuilder CreateScriptBuilder();

        string[] GetTableNames();

        ITableInspector GetTableInspector(string tableName);
    }
}
