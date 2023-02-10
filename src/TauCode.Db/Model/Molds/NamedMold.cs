using TauCode.Db.Model.Interfaces;

namespace TauCode.Db.Model.Molds;

public abstract class NamedMold : Mold, INamedMold
{
    public string Name { get; set; } = null!;
}