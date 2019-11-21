using System.Collections.Generic;

namespace TauCode.Db.Model
{
    public class IndexColumnMold
    {
        public string Name { get; set; }
        public SortDirection SortDirection { get; set; } = SortDirection.Ascending;
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}
