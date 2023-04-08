namespace TauCode.Db.Model.Interfaces;

public interface ITableMold : INamedMold
{
    string? SchemaName { get; set; }
    IList<IColumnMold> Columns { get; set; }
    IList<IConstraintMold> Constraints { get; }
    IList<IIndexMold> Indexes { get; }
}