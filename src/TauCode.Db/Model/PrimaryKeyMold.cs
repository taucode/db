using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TauCode.Db.Model
{
    [DebuggerDisplay("{" + nameof(GetDefaultCaption) + "()}")]
    public class PrimaryKeyMold
    {
        public string Name { get; set; }
        public List<IndexColumnMold> Columns { get; set; } = new List<IndexColumnMold>();

        public string GetDefaultCaption()
        {
            var sb = new StringBuilder();

            var coliumns = string.Join(
                ", ",
                this.Columns
                    .Select(x => x.Name + " " + (x.SortDirection == SortDirection.Ascending ? "ASC" : "DESC")));

            sb.Append($"CONSTRAINT {this.Name} PRIMARY KEY({coliumns})");
            return sb.ToString();
        }
    }
}
