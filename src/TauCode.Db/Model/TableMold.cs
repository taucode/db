using System.Collections.Generic;
using System.Diagnostics;

namespace TauCode.Db.Model
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class TableMold
    {
        public string Name { get; set; }
        public List<ColumnMold> Columns { get; set; } = new List<ColumnMold>();
        public PrimaryKeyMold PrimaryKey { get; set; }
        public List<ForeignKeyMold> ForeignKeys { get; set; } = new List<ForeignKeyMold>();
        public List<IndexMold> Indexes { get; set; } = new List<IndexMold>();
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}
