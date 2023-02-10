namespace TauCode.Db.Model.Interfaces;

public interface ITableMold : INamedMold
{
    IList<IColumnMold> Columns { get; }
    IList<IConstraintMold> Constraints { get; }
    IList<IIndexMold> Indexes { get; }
}