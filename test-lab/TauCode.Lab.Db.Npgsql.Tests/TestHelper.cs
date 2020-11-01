using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TauCode.Db;

namespace TauCode.Lab.Db.Npgsql.Tests
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

        internal static void Purge(this NpgsqlConnection connection)
        {
            var schemata = connection.GetSchemata();

            foreach (var schema in schemata)
            {
                var tableNames = connection.GetTableNames(schema, false);
                foreach (var tableName in tableNames)
                {
                    connection.DropTable(schema, tableName);
                }

                if (schema != NpgsqlToolsLab.DefaultSchemaName)
                {
                    connection.DropSchema(schema);
                }
            }
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
            IDbTableInspector tableInspector = new NpgsqlTableInspectorLab(connection, schemaName, tableName);
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
            command.CommandText = $"SELECT COUNT(*) FROM [{schemaName}].[{tableName}]";
            var count = (int)command.ExecuteScalar();
            return count;
        }
    }
}
