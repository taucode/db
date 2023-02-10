using TauCode.Db.Model.Interfaces;

namespace TauCode.Db.Model.Molds;

public abstract class Mold : IMold
{
    #region IMold Members

    public IDictionary<string, string> Properties { get; } = new Dictionary<string, string>();
    public abstract IMold Clone(bool includeProperties = false);

    #endregion
}