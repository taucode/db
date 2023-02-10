using TauCode.Db.Model.Enums;

namespace TauCode.Db.Model.Interfaces;

public interface IConstraintMold
{
    string Name { get; set; }
    ConstraintType Type { get; }
}