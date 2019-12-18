using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TauCode.Db.Model
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class TableMold : IMold
    {
        public string Name { get; set; }
        public IList<ColumnMold> Columns { get; set; } = new List<ColumnMold>();
        public PrimaryKeyMold PrimaryKey { get; set; }
        public IList<ForeignKeyMold> ForeignKeys { get; set; } = new List<ForeignKeyMold>();
        public IList<IndexMold> Indexes { get; set; } = new List<IndexMold>();
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public IMold Clone(bool includeProperties = false)
        {
            return new TableMold
            {
                Name = this.Name,
                Columns = this.Columns
                    .Select(x => x.CloneColumn(includeProperties))
                    .ToList(),
                PrimaryKey = this.PrimaryKey?.ClonePrimaryKey(includeProperties),
                ForeignKeys = this.ForeignKeys
                    .Select(x => x.CloneForeignKey(includeProperties))
                    .ToList(),
                Indexes = this.Indexes
                    .Select(x => x.CloneIndex(includeProperties))
                    .ToList(),
                Properties = this.ClonePropertiesIfNeeded(includeProperties)
            };
        }
    }
}
