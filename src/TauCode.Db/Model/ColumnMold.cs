using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace TauCode.Db.Model
{
    [DebuggerDisplay("{" + nameof(GetDefaultCaption) + "()}")]
    public class ColumnMold : IDbMold
    {
        public string Name { get; set; }
        public DbTypeMold Type { get; set; } = new DbTypeMold();
        public bool IsNullable { get; set; } = true;
        public ColumnIdentityMold Identity { get; set; }
        public string Default { get; set; }
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();

        public IDbMold Clone(bool includeProperties = false)
        {
            return new ColumnMold
            {
                Name = this.Name,
                Type = this.Type.CloneType(includeProperties),
                IsNullable = this.IsNullable,
                Identity = this.Identity?.CloneColumnIdentity(includeProperties),
                Default = this.Default,
                Properties = this.ClonePropertiesIfNeeded(includeProperties),
            };
        }

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
