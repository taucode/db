using TauCode.Db.Model.Enums;

namespace TauCode.Db.Model.Interfaces;

public interface IConstraintMold : INamedMold
{
    ConstraintType Type { get; }
}