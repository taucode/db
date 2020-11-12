using MySql.Data.MySqlClient;
using System;

namespace TauCode.Db.MySql
{
    public static class MySqlTools
    {
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
