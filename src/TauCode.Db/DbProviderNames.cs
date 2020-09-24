using System;
using System.Linq;

namespace TauCode.Db
{
    public static class DbProviderNames
    {
        public const string SQLite = "SQLite";
        public const string SQLServer = "SQL Server";
        public const string MySQL = "MySQL";
        public const string PostgreSQL = "PostgreSQL";

        public static string[] GetAll() => new[]
        {
            SQLite,
            SQLServer,
            MySQL,
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
