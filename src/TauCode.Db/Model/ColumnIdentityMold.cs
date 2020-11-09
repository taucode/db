using System.Collections.Generic;

namespace TauCode.Db.Model
{
    public class ColumnIdentityMold : IMold
    {
        public ColumnIdentityMold()
        {
        }

        public ColumnIdentityMold(string seed, string increment)
        {
            this.Seed = seed;
            this.Increment = increment;
        }

        public string Seed { get; set; }
        public string Increment { get; set; }
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public IMold Clone(bool includeProperties = false)
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
