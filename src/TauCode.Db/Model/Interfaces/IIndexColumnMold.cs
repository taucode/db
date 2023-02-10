using TauCode.Db.Model.Enums;

namespace TauCode.Db.Model.Interfaces;

public interface IIndexColumnMold : INamedMold
{
    public SortDirection SortDirection { get; set; }
}