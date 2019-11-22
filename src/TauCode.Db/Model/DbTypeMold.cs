using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TauCode.Db.Model
{
    [DebuggerDisplay("{" + nameof(GetDefaultDefinition) + "()}")]
    public class DbTypeMold : IDbMold
    {
        public string Name { get; set; }

        public int? Size { get; set; }

        public int? Precision { get; set; }

        public int? Scale { get; set; }

        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public string GetDefaultDefinition()
        {
            var sb = new StringBuilder();
            sb.Append(this.Name);
            if (this.Size.HasValue)
            {
                sb.Append($"({this.Size.Value})");
            }

            if (this.Precision.HasValue || this.Scale.HasValue)
            {
                sb.AppendFormat(
                    "({0}, {1})",
                    this.Precision.HasValue ? this.Precision.Value.ToString() : "?",
                    this.Scale.HasValue ? this.Scale.Value.ToString() : "?");
            }

            return sb.ToString();
        }
    }
}