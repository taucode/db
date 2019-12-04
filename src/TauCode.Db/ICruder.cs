using System.Collections.Generic;

namespace TauCode.Db
{
    // todo: clean up
    public interface ICruder : IUtility
    {
        //IDbInspector DbInspector { get; }
        IScriptBuilderLab ScriptBuilderLab { get; }
        void InsertRow(string tableName, object row);
        void InsertRows(string tableName, IReadOnlyList<object> rows);
        bool DeleteRow(string tableName, object id);
        dynamic GetRow(string tableName, object id);
        bool UpdateRow(string tableName, object rowUpdate, object id);
        void Reset();
    }
}
