using System.Collections.Generic;

namespace TauCode.Db.Model
{
    public class IndexColumnMold : IMold
    {
        public string Name { get; set; }
        public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
        public IMold Clone(bool includeProperties = false)
        {
            return new IndexColumnMold
            {
                Name = this.Name,
                SortDirection = this.SortDirection,
                Properties = this.ClonePropertiesIfNeeded(includeProperties),
            };
        }
    }
}
