﻿using System.Collections.Generic;

namespace TauCode.Db.Model
{
    public class LiteralMold
    {
        public string Value { get; set; }
        public IDictionary<string, string> Properties { get; set; } = new Dictionary<string, string>();
    }
}
