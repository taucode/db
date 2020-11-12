using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TauCode.Db.Extensions;

namespace TauCode.Db.Npgsql.Tests
{
    internal static class TestHelper
    {
        internal const string ConnectionString = @"User ID=postgres;Password=1234;Host=localhost;Port=5432;Database=my_tests";

        internal static NpgsqlConnection CreateConnection()
        {
            var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        internal static void PurgeDatabase(this NpgsqlConnection connection)
        {
            new NpgsqlSchemaExplorer(connection).PurgeDatabase();
        }

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

        internal static IReadOnlyDictionary<string, object> LoadRow(
            NpgsqlConnection connection,
            string schemaName,
            string tableName,
            object id)
        {
            IDbTableInspector tableInspector = new NpgsqlTableInspector(connection, schemaName, tableName);
            var table = tableInspector.GetTable();
            var pkColumnName = table.GetPrimaryKeySingleColumn().Name;

            using var command = connection.CreateCommand();
            command.CommandText = $@"
SELECT
    *
FROM
    ""{schemaName}"".""{tableName}""
WHERE
    ""{pkColumnName}"" = @p_id
";
            command.Parameters.AddWithValue("p_id", id);
            using var reader = command.ExecuteReader();

            var read = reader.Read();
            if (!read)
            {
                return null;
            }

            var dictionary = new Dictionary<string, object>();

            for (var i = 0; i < reader.FieldCount; i++)
            {
                var fieldName = reader.GetName(i);
                var value = reader[fieldName];

                if (value == DBNull.Value)
                {
                    value = null;
                }

                dictionary[fieldName] = value;
            }

            return dictionary;
        }

        internal static long GetLastIdentity(this NpgsqlConnection connection, string schemaName, string tableName, string columnName)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT currval(pg_get_serial_sequence('\"{schemaName}\".\"{tableName}\"', '{columnName}'))";
            return (long)command.ExecuteScalar();
        }

        internal static int GetTableRowCount(NpgsqlConnection connection, string schemaName, string tableName)
        {
            using var command = connection.CreateCommand();
            command.CommandText = @$"SELECT COUNT(*) FROM ""{schemaName}"".""{tableName}""";
            var count = (int)(long)command.ExecuteScalar();
            return count;
        }

        internal static IReadOnlyList<string> GetTableNames(this NpgsqlConnection connection, string schemaName, bool independentFirst)
            => new NpgsqlSchemaExplorer(connection).GetTableNames(schemaName, independentFirst);

        internal static void CreateSchema(this NpgsqlConnection connection, string schemaName)
            => new NpgsqlSchemaExplorer(connection).CreateSchema(schemaName);
        
        internal static void DropTable(this NpgsqlConnection connection, string schemaName, string tableName)
            => new NpgsqlSchemaExplorer(connection).DropTable(schemaName, tableName);
    }
}
