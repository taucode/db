using System.Collections.Generic;

namespace TauCode.Db.MySql
{
    public static class MySqlTools
    {
        internal static string SafeGetValue(this IDictionary<string, string> dictionary, string key)
        {
            return ((Dictionary<string, string>)dictionary).GetValueOrDefault(key);
        }
    }
}
