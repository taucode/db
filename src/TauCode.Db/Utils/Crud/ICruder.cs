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
        void InsertRow(string tableName, object row);
        bool DeleteRow(string tableName, object id);
        List<dynamic> GetRows(IDbCommand command);
        dynamic GetRow(string tableName, object id);
        bool UpdateRow(string tableName, object rowUpdate, object id);
    }
}
