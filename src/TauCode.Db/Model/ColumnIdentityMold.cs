using System.Collections.Generic;

namespace TauCode.Db.Model
{
    public class ColumnIdentityMold : IDbMold
    {
        public string Seed { get; set; }
        public string Increment { get; set; }
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}
