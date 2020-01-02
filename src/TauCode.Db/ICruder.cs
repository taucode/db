using System.Collections.Generic;

namespace TauCode.Db
{
    public interface ICruder : IUtility
    {
        IScriptBuilder ScriptBuilder { get; }
        ITableValuesConverter GetTableValuesConverter(string tableName);
        void ResetTableValuesConverters();
        void InsertRow(string tableName, object row);
        void InsertRows(string tableName, IReadOnlyList<object> rows);
        dynamic GetRow(string tableName, object id);
        IList<dynamic> GetAllRows(string tableName);
        bool UpdateRow(string tableName, object rowUpdate, object id);
        bool DeleteRow(string tableName, object id);
    }
}
