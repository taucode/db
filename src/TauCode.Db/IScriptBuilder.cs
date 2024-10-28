using TauCode.Db.Model.Interfaces;

namespace TauCode.Db;

public interface IScriptBuilder : IUtility
{
    string BuildSelectAllRowsScript(ITableMold tableMold, Func<string, bool>? fieldSelector = null);
    string BuildInsertScript(ITableMold tableMold, Func<string, bool>? fieldSelector, out IDictionary<string, IParameterMold> parameterMapping);
}
