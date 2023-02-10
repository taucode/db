using TauCode.Db.Model.Interfaces;

namespace TauCode.Db.Model.Molds;

public class ColumnMold : NamedMold, IColumnMold
{
    #region IColumnMold Members

    public IDbTypeMold Type { get; set; } = null!;
    public bool IsNullable { get; set; } = true;
    public IColumnIdentityMold? Identity { get; set; }

    #endregion

    #region Overridden

    public override IMold Clone(bool includeProperties = false)
    {
        var clonedColumnMold = new ColumnMold
        {
            Name = this.Name,
            Type = (IDbTypeMold)this.Type.Clone(includeProperties),
            IsNullable = this.IsNullable,
            Identity = (IColumnIdentityMold?)(this.Identity?.Clone(includeProperties)),
        };

        clonedColumnMold.CopyPropertiesFrom(this);

        return clonedColumnMold;
    }

    #endregion

    // todo deal with this
    //public string GetDefaultCaption()
    //{
    //    var sb = new StringBuilder();
    //    sb.Append($"{this.Name} ");

    //    if (this.Type != null)
    //    {
    //        sb.Append($"{this.Type.GetDefaultDefinition()} ");
    //        sb.Append(this.IsNullable ? "NULL" : "NOT NULL");
    //    }

    //    return sb.ToString();
    //}
}