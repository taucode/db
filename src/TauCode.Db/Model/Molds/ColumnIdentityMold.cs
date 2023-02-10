using TauCode.Db.Model.Interfaces;

namespace TauCode.Db.Model.Molds;

public class ColumnIdentityMold : Mold, IColumnIdentityMold
{
    #region IColumnIdentityMold Members

    public string? Seed { get; set; }
    public string Increment { get; set; } = null!;

    #endregion

    #region Overridden

    public override IMold Clone(bool includeProperties = false)
    {
        var clonedColumnIdentityMold = new ColumnIdentityMold
        {
            Seed = this.Seed,
            Increment = this.Increment,
        };

        clonedColumnIdentityMold.CopyPropertiesFrom(this);

        return clonedColumnIdentityMold;
    }

    #endregion
}