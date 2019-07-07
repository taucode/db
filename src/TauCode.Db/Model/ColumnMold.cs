using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TauCode.Db.Model
{
    [DebuggerDisplay("{" + nameof(GetDefaultCaption) + "()}")]
    public class ColumnMold
    {
        public string Name { get; set; }
        public DbTypeMold Type { get; set; }
        public bool IsNullable { get; set; } = true;
        public ColumnIdentityMold Identity { get; set; }
        public Dictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public string GetDefaultCaption()
        {
            var sb = new StringBuilder();
            sb.Append($"{this.Name} ");

            if (this.Type != null)
            {
                sb.Append($"{this.Type.GetDefaultDefinition()} ");
                sb.Append(this.IsNullable ? "NULL" : "NOT NULL");
            }

            return sb.ToString();
        }
    }
}
