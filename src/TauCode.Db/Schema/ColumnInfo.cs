using System.Collections.Generic;

namespace TauCode.Db.Schema
{
    public class ColumnInfo
    {
        public string Name { get; set; }
        public string TypeName { get; set; }
        public bool IsNullable { get; set; }
        public int? Size { get; set; }
        public int? Precision { get; set; }
        public int? Scale { get; set; }
        public Dictionary<string, string> Additional { get; set; } = new Dictionary<string, string>();
    }
}
