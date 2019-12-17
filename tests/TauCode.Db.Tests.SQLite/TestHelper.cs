using System;
using System.Data.SQLite;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Tests.SQLite
{
    internal static class TestHelper
    {
        /// <summary>
        /// Creates temporary .sqlite file and returns a SQLite connection string for this file.
        /// </summary>
        /// <returns>
        /// Tuple with two strings. Item1 is temporary file path, Item2 is connection string.
        /// </returns>
        internal static Tuple<string, string> CreateSQLiteConnectionString()
        {
            var tempDbFilePath = FileExtensions.CreateTempFilePath("zunit", ".sqlite");
            SQLiteConnection.CreateFile(tempDbFilePath);

            var connectionString = $"Data Source={tempDbFilePath};Version=3;";

            return Tuple.Create(tempDbFilePath, connectionString);
        }
    }
}
