namespace TauCode.Db;

public interface ICruder : IDataUtility
{
    IValueConverter ValueConverter { get; set; }

    IExplorer Explorer { get; }

    IScriptBuilder ScriptBuilder { get; }

    IRowSet GetAllRows(
        string? schemaName,
        string tableName,
        Func<string, bool>? fieldSelector = null);

    void InsertRows(
        string? schemaName,
        string tableName,
        IRowSet rows,
        Func<string, bool>? fieldSelector = null);
}
