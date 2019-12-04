using System.Collections.Generic;

namespace TauCode.Db
{
    public interface IDbInspector : IUtility
    {
        //IDbConnection Connection { get; }

        //IScriptBuilder CreateScriptBuilder();

        IReadOnlyList<string> GetTableNames(bool? independentFirst = null);

        ITableInspector GetTableInspector(string tableName);
    }
}
