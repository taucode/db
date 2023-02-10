using TauCode.Db.Model.Enums;

namespace TauCode.Db.Model.Interfaces;

internal interface IIndexColumnMold : IMold
{
    public string Name { get; set; }
    public SortDirection SortDirection { get; set; }
}