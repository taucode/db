using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Tests
{
    internal static class TestHelper
    {
        internal static void ExecuteNonQuery(this IDbConnection connection, string sql)
        {
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        internal static IEnumerable<T> Check<T>(this IEnumerable<T> collection, Func<T, bool> predicate)
        {
            foreach (var item in collection)
            {
                if (!predicate(item))
                {
                    Assert.Fail("Check failed");
                }
            }

            return collection;
        }

        internal static void AddParameterWithValue(this IDbCommand command, string parameterName, object parameterValue)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            command.Parameters.Add(parameter);
        }

        internal static void WriteDiff(string actual, string expected, string directory, string fileExtension)
        {
            var actualFileName = $"0-actual.{fileExtension}";
            var expectedFileName = $"1-expected.{fileExtension}";

            var actualFilePath = Path.Combine(directory, actualFileName);
            var expectedFilePath = Path.Combine(directory, expectedFileName);

            File.WriteAllText(actualFilePath, actual, Encoding.UTF8);
            File.WriteAllText(expectedFilePath, expected, Encoding.UTF8);
        }

        internal static IDbConnection CreateTempSQLiteDatabase()
        {
            var path = FileExtensions.CreateTempFilePath("zunit", ".sqlite");
            SQLiteConnection.CreateFile(path);
            var connectionString = TestHelper.BuildSQLiteConnectionString(path);
            var connection = new SQLiteConnection(connectionString);
            connection.Open();
            return connection;
        }

        internal static string BuildSQLiteConnectionString(string path)
        {
            return $"Data Source={path};Version=3;";
        }

        internal static string GetSQLiteDatabasePath(IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        internal static string GetSQLiteDatabasePath(string sqliteConnectionString)
        {
            var match = Regex.Match(sqliteConnectionString, "Data Source=([^;]*);");
            var result = match.Groups[1].ToString();

            return result;
        }
    }
}
