using System.Collections.Generic;

namespace TauCode.Db.Model
{
    public class LiteralMold : IDbMold
    {
        public string Value { get; set; }
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public IDbMold Clone(bool includeProperties = false)
        {
            return new LiteralMold
            {
                Value = this.Value,
                Properties = this.ClonePropertiesIfNeeded(includeProperties),
            };
        }
    }
}
