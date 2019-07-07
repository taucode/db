using System.Collections.Generic;
using System.Diagnostics;

namespace TauCode.Db.Model
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class IndexMold
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public List<string> ColumnNames { get; set; } = new List<string>();
        public bool IsUnique { get; set; }
    }
}
