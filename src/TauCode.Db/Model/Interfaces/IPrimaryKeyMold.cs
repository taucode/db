namespace TauCode.Db.Model.Interfaces;

public interface IPrimaryKeyMold : IConstraintMold
{
    IList<IIndexColumnMold> Columns { get; set; }
}