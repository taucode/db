using System.Diagnostics;
using TauCode.Db.Model.Enums;
using TauCode.Db.Model.Interfaces;

namespace TauCode.Db.Model;

[DebuggerDisplay($"{{{nameof(Name)}}} {{{nameof(SortDirection)}}}")]
public class IndexColumnMold : IIndexColumnMold
{
    #region IIndexColumnMold Members

    public string Name { get; set; } = null!;

    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

    #endregion

    #region Imold Members

    public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    public IMold Clone(bool includeProperties = false)
    {
        return new IndexColumnMold
        {
            Name = this.Name,
            SortDirection = this.SortDirection,
            Properties = this.ClonePropertiesIfNeeded(includeProperties),
        };
    }

    #endregion
}