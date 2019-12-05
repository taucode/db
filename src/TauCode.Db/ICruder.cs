using System.Collections.Generic;

namespace TauCode.Db
{
    public interface ICruder : IUtility
    {
        IScriptBuilderLab ScriptBuilderLab { get; }
        void InsertRow(string tableName, object row);
        void InsertRows(string tableName, IReadOnlyList<object> rows);
        dynamic GetRow(string tableName, object id);
        IList<dynamic> GetAllRows(string tableName);
        bool UpdateRow(string tableName, object rowUpdate, object id);
        bool DeleteRow(string tableName, object id);
    }
}
