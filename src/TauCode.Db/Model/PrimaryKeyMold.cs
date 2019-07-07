using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TauCode.Db.Model
{
    [DebuggerDisplay("{" + nameof(GetDefaultCaption) + "()}")]
    public class PrimaryKeyMold
    {
        public string Name { get; set; }
        public List<string> ColumnNames { get; set; } = new List<string>();

        public string GetDefaultCaption()
        {
            var sb = new StringBuilder();
            var columnNames = string.Join(", ", this.ColumnNames);
            sb.Append($"CONSTRAINT {this.Name} PRIMARY KEY({columnNames})");
            return sb.ToString();
        }
    }
}
