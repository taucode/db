using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TauCode.Db.Model
{
    [DebuggerDisplay("{" + nameof(GetDefaultCaption) + "()}")]
    public class ForeignKeyMold
    {
        public string Name { get; set; }
        public List<string> ColumnNames { get; set; } = new List<string>();
        public string ReferencedTableName { get; set; }
        public List<string> ReferencedColumnNames { get; set; } = new List<string>();
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public string GetDefaultCaption()
        {
            var sb = new StringBuilder();
            var columnNames = string.Join(", ", this.ColumnNames);
            var referencedColumnNames = string.Join(", ", this.ReferencedColumnNames);

            sb.Append("CONSTRAINT ");
            if (!string.IsNullOrEmpty(this.Name))
            {
                sb.Append(this.Name);
                sb.Append(" ");
            }

            sb.Append($"FOREIGN KEY({columnNames}) REFERENCES {this.ReferencedTableName}({referencedColumnNames})");
            return sb.ToString();
        }
    }
}
