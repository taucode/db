using TauCode.Db.Model.Enums;
using TauCode.Db.Model.Interfaces;

namespace TauCode.Db.Model.Molds;

public abstract class ConstraintMold : NamedMold, IConstraintMold
{
    #region IConstraintMold Members

    public string? SchemaName { get; set; }
    public string TableName { get; set; } = null!;
    public abstract ConstraintType Type { get; }

    #endregion
}