using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;

namespace TauCode.Db.MySql
{
    public static class MySqlTools
    {
        //internal static string SafeGetValue(this IDictionary<string, string> dictionary, string key)
        //{
        //    return ((Dictionary<string, string>)dictionary).GetValueOrDefault(key);
        //}

        internal static void CheckConnectionArgument(
            MySqlConnection connection,
            string connectionArgumentName = "connection")
        {
            if (connection.Database == null)
            {
                throw new ArgumentException($"'{nameof(MySqlConnection.Database)}' property of connection is null.", connectionArgumentName);
            }
        }
    }
}
