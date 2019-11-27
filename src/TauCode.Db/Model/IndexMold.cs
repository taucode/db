using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace TauCode.Db.Model
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class IndexMold : IDbMold
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public List<IndexColumnMold> Columns { get; set; } = new List<IndexColumnMold>();
        public bool IsUnique { get; set; }
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public IDbMold Clone(bool includeProperties = false)
        {
            return new IndexMold
            {
                Name = this.Name,
                TableName = this.TableName,
                Columns = this.Columns
                    .Select(x => x.CloneIndexColumn(includeProperties))
                    .ToList(),
            };
        }
    }
}
