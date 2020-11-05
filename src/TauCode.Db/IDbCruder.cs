using System;
using System.Collections.Generic;

namespace TauCode.Db
{
    public interface IDbCruder : IDbUtility
    {
        string SchemaName { get; }
        IDbScriptBuilder ScriptBuilder { get; }
        IDbTableValuesConverter GetTableValuesConverter(string tableName);
        void ResetTables();
        void InsertRow(string tableName, object row, Func<string, bool> propertySelector = null);
        void InsertRows(string tableName, IReadOnlyList<object> rows, Func<string, bool> propertySelector = null);
        Action<string, object, int> RowInsertedCallback { get; set; }
        dynamic GetRow(string tableName, object id, Func<string, bool> columnSelector = null);
        IList<dynamic> GetAllRows(string tableName, Func<string, bool> columnSelector = null);
        bool UpdateRow(string tableName, object rowUpdate, Func<string, bool> propertySelector = null);
        bool DeleteRow(string tableName, object id);
    }
}
