﻿using System;
using System.Data.SQLite;
using System.IO;
using System.Text;
using TauCode.Extensions;

namespace TauCode.Db.Tests.SQLite
{
    internal static class TestHelper
    {
        internal static string GetResourceText(string fileName) =>
            typeof(TestHelper).Assembly.GetResourceText(fileName, true);

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

        internal static Tuple<string, string> CreateSQLiteConnectionString()
        {
            var tempDbFilePath = FileExtensions.CreateTempFilePath("zunit", ".sqlite");
            SQLiteConnection.CreateFile(tempDbFilePath);

            var connectionString = $"Data Source={tempDbFilePath};Version=3;";

            return Tuple.Create(tempDbFilePath, connectionString);
        }
    }
}
