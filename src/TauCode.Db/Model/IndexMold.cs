using System.Collections.Generic;
using System.Diagnostics;

namespace TauCode.Db.Model
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class IndexMold
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public List<IndexColumnMold> Columns { get; set; } = new List<IndexColumnMold>();
        public bool IsUnique { get; set; }
    }
}
