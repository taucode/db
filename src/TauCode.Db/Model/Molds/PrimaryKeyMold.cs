using TauCode.Db.Model.Enums;
using TauCode.Db.Model.Interfaces;

namespace TauCode.Db.Model.Molds;

public class PrimaryKeyMold : ConstraintMold, IPrimaryKeyMold
{
    #region IPrimaryKeyMold Members

    public IList<IIndexColumnMold> Columns { get; } = new List<IIndexColumnMold>();

    #endregion

    #region Overridden

    public override ConstraintType Type => ConstraintType.PrimaryKey;

    public override IMold Clone(bool includeProperties = false)
    {
        var clonedPrimaryKeyMold = new PrimaryKeyMold
        {
            Name = this.Name,
            SchemaName = this.SchemaName,
            TableName = this.TableName,
        };

        var clonedColumns = (List<IIndexColumnMold>)clonedPrimaryKeyMold.Columns;
        clonedColumns.AddRange(this.Columns.Select(x => (IIndexColumnMold)x.Clone(includeProperties)));

        if (includeProperties)
        {
            clonedPrimaryKeyMold.CopyPropertiesFrom(this);
        }

        return clonedPrimaryKeyMold;
    }

    #endregion

    // todo deal with this
    //public string GetDefaultCaption()
    //{
    //    var sb = new StringBuilder();

    //    var columns = string.Join(
    //        ", ",
    //        this.Columns);

    //    sb.Append($"CONSTRAINT {this.Name} PRIMARY KEY({columns})");
    //    return sb.ToString();
    //}
}