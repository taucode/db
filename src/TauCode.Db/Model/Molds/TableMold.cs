using System.Diagnostics;
using TauCode.Db.Model.Interfaces;

namespace TauCode.Db.Model.Molds;

[DebuggerDisplay("{" + nameof(Name) + "}")]
public class TableMold : NamedMold, ITableMold
{
    #region ITableMold Members

    public string? SchemaName { get; set; }
    public IList<IColumnMold> Columns { get; set; } = new List<IColumnMold>();
    public IList<IConstraintMold> Constraints { get; } = new List<IConstraintMold>();
    public IList<IIndexMold> Indexes { get; } = new List<IIndexMold>();

    #endregion

    #region Overridden

    public override IMold Clone(bool includeProperties = false)
    {
        throw new NotImplementedException();
    }

    #endregion
}