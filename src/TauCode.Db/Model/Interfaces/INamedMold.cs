namespace TauCode.Db.Model.Interfaces;

public interface INamedMold : IMold
{
    string Name { get; set; }
}