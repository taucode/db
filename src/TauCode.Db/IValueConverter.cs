using TauCode.Db.Model.Interfaces;

namespace TauCode.Db;

public interface IValueConverter
{
    object ConvertFromDb(string? schemaName, string tableName, IColumnMold columnMold, object value);
    object ConvertToDb(string? schemaName, string tableName, IColumnMold columnMold, object? value);
}