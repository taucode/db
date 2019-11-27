using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TauCode.Db.Model
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class TableMold : IDbMold
    {
        public string Name { get; set; }
        public List<ColumnMold> Columns { get; set; } = new List<ColumnMold>();
        public PrimaryKeyMold PrimaryKey { get; set; }
        public List<ForeignKeyMold> ForeignKeys { get; set; } = new List<ForeignKeyMold>();
        public List<IndexMold> Indexes { get; set; } = new List<IndexMold>();
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public IDbMold Clone(bool includeProperties = false)
        {
            return new TableMold
            {
                Name = this.Name,
                Columns = this.Columns
                    .Select(x => (ColumnMold)x.Clone(includeProperties))
                    .ToList(),
                PrimaryKey = (PrimaryKeyMold)this.PrimaryKey?.Clone(includeProperties),
                ForeignKeys = this.ForeignKeys
                    .Select(x => (ForeignKeyMold)x.Clone())
                    .ToList(),
                Indexes = this.Indexes
                    .Select(x => (IndexMold)x.Clone(includeProperties))
                    .ToList(),
                Properties = this.ClonePropertiesIfNeeded(includeProperties)
            };
        }
    }
}
