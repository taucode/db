using System.Collections.Generic;
using System.Data;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Inspection;

namespace TauCode.Db.Utils.Crud
{
    public interface ICruder
    {
        IDbInspector DbInspector { get; }
        IScriptBuilder ScriptBuilder { get; }
        ITableInspector GetTableInspector(IDbConnection connection, string tableName);

        void InsertRow(IDbConnection connection, string tableName, object row);
        bool DeleteRow(IDbConnection connection, string tableName, object id);
        List<dynamic> GetRows(IDbCommand command);
        dynamic GetRow(IDbConnection connection, string tableName, object id);
        bool UpdateRow(IDbConnection connection, string tableName, object rowUpdate, object id);
    }
}
