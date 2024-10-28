using TauCode.Db.Model.Enums;

namespace TauCode.Db.Model.Interfaces;

public interface IConstraintMold : INamedMold
{
    string? SchemaName { get; set; }
    string TableName { get; set; }
    ConstraintType Type { get; }
}