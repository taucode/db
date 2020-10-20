using System;
using System.Collections.Generic;

namespace TauCode.Db
{
    public interface IDbCruder : IDbUtility
    {
        string SchemaName { get; }
        IDbScriptBuilder ScriptBuilder { get; }
        IDbTableValuesConverter GetTableValuesConverter(string tableName);
        void ResetTableValuesConverters();
        void InsertRow(string tableName, object row, Func<string, bool> propertySelector); // todo: add 'columnSelector = null' default after all ut-s are green
        void InsertRows(string tableName, IReadOnlyList<object> rows, Func<string, bool> propertySelector); // todo: add 'columnSelector = null' default after all ut-s are green
        Action<string, object, int> RowInsertedCallback { get; set; }
        dynamic GetRow(string tableName, object id);
        IList<dynamic> GetAllRows(string tableName);
        bool UpdateRow(string tableName, object rowUpdate, object id);
        bool DeleteRow(string tableName, object id);
        void DeleteRows(string tableName, object[] ids);
    }
}
