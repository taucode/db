using System.Collections.Generic;
using System.Linq;

namespace TauCode.Db.Model
{
    public class DbMold : IMold
    {
        public string DbProviderName { get; set; }
        public IList<TableMold> Tables { get; set; }= new List<TableMold>();

        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public IMold Clone(bool includeProperties = false)
        {
            return new DbMold
            {
                DbProviderName = this.DbProviderName,
                Tables = this.Tables
                    .Select(x => x.CloneTable(includeProperties))
                    .ToList(),
                Properties = this.ClonePropertiesIfNeeded(includeProperties),
            };
        }
    }
}
