using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using TauCode.Db.SqlServer;
using TauCode.Extensions;

namespace TauCode.Db.Tests.SqlServer
{
    internal static class TestHelper
    {
        internal const string ConnectionString = @"Server=.\mssqltest;Database=rho.test;Trusted_Connection=True;";

        internal static void WriteDiff(string actual, string expected, string directory, string fileExtension, string reminder)
        {
            if (reminder != "to" + "do")
            {
                throw new InvalidOperationException("don't forget this call with mark!");
            }

            fileExtension = fileExtension.Replace(".", "");

            var actualFileName = $"0-actual.{fileExtension}";
            var expectedFileName = $"1-expected.{fileExtension}";

            var actualFilePath = Path.Combine(directory, actualFileName);
            var expectedFilePath = Path.Combine(directory, expectedFileName);

            File.WriteAllText(actualFilePath, actual, Encoding.UTF8);
            File.WriteAllText(expectedFilePath, expected, Encoding.UTF8);
        }

        internal static string GetResourceText(string fileName) =>
            typeof(TestHelper).Assembly.GetResourceText(fileName, true);

        internal static void SafeDropTable(this IDbConnection connection, string tableName)
        {
            var dbInspector = new SqlServerInspector(connection);
            var tableNames = dbInspector.GetTableNames();
            if (tableNames.Contains(tableName, StringComparer.InvariantCultureIgnoreCase))
            {
                var command = connection.CreateCommand();
                command.CommandText = $"DROP TABLE {tableName}";
                command.ExecuteNonQuery();
            }
        }
    }
}
