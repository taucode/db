using System.Diagnostics;
using TauCode.Db.Model.Enums;
using TauCode.Db.Model.Interfaces;

namespace TauCode.Db.Model.Molds;

[DebuggerDisplay($"{{{nameof(Name)}}} {{{nameof(SortDirection)}}}")]
public class IndexColumnMold : NamedMold, IIndexColumnMold
{
    #region IIndexColumnMold Members

    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

    #endregion

    #region Overridden

    public override IMold Clone(bool includeProperties = false)
    {
        var clonedColumn = new IndexColumnMold
        {
            Name = this.Name,
            SortDirection = this.SortDirection,
        };

        if (includeProperties)
        {
            clonedColumn.CopyPropertiesFrom(this);
        }

        return clonedColumn;
    }

    #endregion
}