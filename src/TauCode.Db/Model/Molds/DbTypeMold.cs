using TauCode.Db.Model.Interfaces;

namespace TauCode.Db.Model.Molds;

public class DbTypeMold : NamedMold, IDbTypeMold
{
    #region IDbTypeMold Members

    public int? Size { get; set; }

    public int? Precision { get; set; }

    public int? Scale { get; set; }

    #endregion

    #region Overridden

    public override IMold Clone(bool includeProperties = false)
    {
        var clonedDbTypeMold = new DbTypeMold
        {
            Name = this.Name,
            Size = this.Size,
            Precision = this.Precision,
            Scale = this.Scale,
        };

        clonedDbTypeMold.CopyPropertiesFrom(this);

        return clonedDbTypeMold;
    }

    #endregion

    // todo deal with this
    //public string GetDefaultDefinition()
    //{
    //    var sb = new StringBuilder();
    //    sb.Append(this.Name);
    //    if (this.Size.HasValue)
    //    {
    //        sb.Append($"({this.Size.Value})");
    //    }

    //    if (this.Precision.HasValue || this.Scale.HasValue)
    //    {
    //        sb.AppendFormat(
    //            "({0}, {1})",
    //            this.Precision.HasValue ? this.Precision.Value.ToString() : "?",
    //            this.Scale.HasValue ? this.Scale.Value.ToString() : "?");
    //    }

    //    return sb.ToString();
    //}
}