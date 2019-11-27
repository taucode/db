using System.Collections.Generic;

namespace TauCode.Db.Model
{
    public class ColumnIdentityMold : IDbMold
    {
        public string Seed { get; set; }
        public string Increment { get; set; }
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public IDbMold Clone(bool includeProperties = false)
        {
            return new ColumnIdentityMold
            {
                Seed = this.Seed,
                Increment = this.Increment,
                Properties = this.ClonePropertiesIfNeeded(includeProperties),
            };
        }
    }
}
