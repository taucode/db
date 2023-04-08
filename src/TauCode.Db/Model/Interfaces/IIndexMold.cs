namespace TauCode.Db.Model.Interfaces;

public interface IIndexMold : INamedMold
{
    string? SchemaName { get; set; }
    string TableName { get; set; }
    IList<IIndexColumnMold> Columns { get; }
    bool IsUnique { get; set; }
}