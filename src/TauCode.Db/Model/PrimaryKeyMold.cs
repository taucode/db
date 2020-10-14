using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace TauCode.Db.Model
{
    [DebuggerDisplay("{" + nameof(GetDefaultCaption) + "()}")]
    public class PrimaryKeyMold : IMold, IConstraint
    {
        public string Name { get; set; }
        public IList<string> Columns { get; set; } = new List<string>();
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public IMold Clone(bool includeProperties = false)
        {
            return new PrimaryKeyMold
            {
                Name = this.Name,
                Columns = this.Columns.ToList(),
                Properties = this.ClonePropertiesIfNeeded(includeProperties),
            };
        }

        public string GetDefaultCaption()
        {
            var sb = new StringBuilder();

            var columns = string.Join(
                ", ",
                this.Columns);

            sb.Append($"CONSTRAINT {this.Name} PRIMARY KEY({columns})");
            return sb.ToString();
        }
    }
}
