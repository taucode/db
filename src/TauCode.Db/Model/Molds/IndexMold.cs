using System.Diagnostics;
using TauCode.Db.Model.Interfaces;

namespace TauCode.Db.Model.Molds;

[DebuggerDisplay("{" + nameof(Name) + "}")]
public class IndexMold : NamedMold, IIndexMold
{
    #region IIndexMold Members

    public string? SchemaName { get; set; }
    public string TableName { get; set; } = null!;
    public IList<IIndexColumnMold> Columns { get; } = new List<IIndexColumnMold>();
    public bool IsUnique { get; set; }

    #endregion

    #region Overridden

    public override IMold Clone(bool includeProperties = false)
    {
        var clonedIndex = new IndexMold
        {
            SchemaName = this.SchemaName,
            Name = this.Name,
            TableName = this.TableName,
            IsUnique = this.IsUnique,
        };

        var clonedColumns = (List<IIndexColumnMold>)clonedIndex.Columns;
        clonedColumns.AddRange(this.Columns.Select(x => (IIndexColumnMold)x.Clone(includeProperties)));

        if (includeProperties)
        {
            clonedIndex.CopyPropertiesFrom(this);
        }

        return clonedIndex;
    }

    #endregion
}