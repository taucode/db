using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TauCode.Db.Model
{
    [DebuggerDisplay("{" + nameof(GetDefaultCaption) + "()}")]
    public class ForeignKeyMold : IMold, IConstraint
    {
        public string Name { get; set; }
        public IList<string> ColumnNames { get; set; } = new List<string>();
        public string ReferencedTableName { get; set; }
        public IList<string> ReferencedColumnNames { get; set; } = new List<string>();
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public IMold Clone(bool includeProperties = false)
        {
            return new ForeignKeyMold
            {
                Name = this.Name,
                ColumnNames = this.ColumnNames.ToList(),
                ReferencedTableName = this.ReferencedTableName,
                ReferencedColumnNames = this.ReferencedColumnNames.ToList(),
                Properties = this.ClonePropertiesIfNeeded(includeProperties),
            };
        }

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
