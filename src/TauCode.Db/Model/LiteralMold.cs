using System.Collections.Generic;

namespace TauCode.Db.Model
{
    public class LiteralMold : IMold
    {
        public string Value { get; set; }
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public IMold Clone(bool includeProperties = false)
        {
            return new LiteralMold
            {
                Value = this.Value,
                Properties = this.ClonePropertiesIfNeeded(includeProperties),
            };
        }
    }
}
