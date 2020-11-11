using System.Collections.Generic;

namespace TauCode.Lab.Db.MySql
{
    public static class MySqlToolsLab
    {
        internal static string SafeGetValue(this IDictionary<string, string> dictionary, string key)
        {
            return ((Dictionary<string, string>)dictionary).GetValueOrDefault(key);
        }
    }
}
