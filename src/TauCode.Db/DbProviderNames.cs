using System;
using System.Linq;

namespace TauCode.Db
{
    public static class DbProviderNames
    {
        public const string Ansi = "ANSI";
        public const string SQLite = "SQLite";
        public const string SqlServer = "SqlServer";
        public const string MySql = "MySql";
        public const string PostgreSQL = "PostgreSQL";

        public static string[] GetAll() => new[]
        {
            Ansi,
            SQLite,
            SqlServer,
            MySql,
            PostgreSQL,
        };

        public static string Find(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return GetAll().FirstOrDefault(x => string.Equals(x, name, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
