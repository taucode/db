﻿using System.Collections.Generic;
using System.Diagnostics;

namespace TauCode.Db.Model
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class IndexMold : IDbMold
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public List<IndexColumnMold> Columns { get; set; } = new List<IndexColumnMold>();
        public bool IsUnique { get; set; }
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}
