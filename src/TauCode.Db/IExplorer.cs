using TauCode.Db.Model.Interfaces;

namespace TauCode.Db;

public interface IExplorer : IDataUtility
{
    IReadOnlyList<string> GetSchemaNames();
    IReadOnlyList<string> GetTableNames(string? schemaName);
    ITableMold GetTable(string? schemaName, string tableName, bool getConstraints = true, bool getIndexes = true);
}
