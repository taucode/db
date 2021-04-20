using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TauCode.Db.Extensions;

namespace TauCode.Db.MySql.Tests
{
    internal static class TestHelper
    {
        internal const string ConnectionString = @"Server=localhost;Database=foo;Uid=root;Pwd=1234;";

        internal static MySqlConnection CreateConnection()
        {
            using var connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        internal static void PurgeDatabase(this MySqlConnection connection)
        {
            new MySqlSchemaExplorer(connection).DropAllSchemas();

            connection.CreateSchema("foo");

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
            MySqlConnection connection,
            string tableName,
            object id)
        {
            IDbTableInspector tableInspector = new MySqlTableInspector(connection, tableName);
            var table = tableInspector.GetTable();
            var pkColumnName = table.GetPrimaryKeySingleColumn().Name;

            var schemaName = connection.Database;

            using var command = connection.CreateCommand();
            command.CommandText = $@"
SELECT
    *
FROM
    `{schemaName}`.`{tableName}`
WHERE
    `{pkColumnName}` = @p_id
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

        internal static ulong GetLastIdentity(this MySqlConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT @@IDENTITY";
            return (ulong)command.ExecuteScalar();
        }

        internal static IReadOnlyList<string> GetTableNames(this MySqlConnection connection, string schemaName, bool independentFirst)
            => new MySqlSchemaExplorer(connection).GetTableNames(schemaName, independentFirst);

        internal static long GetTableRowCount(MySqlConnection connection, string schemaName, string tableName)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(*) FROM `{schemaName}`.`{tableName}`";
            var count = (long)command.ExecuteScalar();
            return count;
        }

        internal static MySqlConnection CreateConnection(string schemaName)
        {
            if (schemaName == null)
            {
                var connectionString = $@"Server=localhost;Uid=root;Pwd=1234;";
                var connection = new MySqlConnection(connectionString);
                connection.Open();
                return connection;
            }
            else
            {
                var connectionString = $@"Server=localhost;Database={schemaName};Uid=root;Pwd=1234;";
                var connection = new MySqlConnection(connectionString);
                connection.Open();
                return connection;
            }
        }

        internal static void DropSchema(this MySqlConnection connection, string schemaName)
        {
            new MySqlSchemaExplorer(connection).DropSchema(schemaName);
        }

        internal static void CreateSchema(this MySqlConnection connection, string schemaName)
        {
            new MySqlSchemaExplorer(connection).CreateSchema(schemaName);
        }

        internal static bool SchemaExists(this MySqlConnection connection, string schemaName)
        {
            return new MySqlSchemaExplorer(connection).SchemaExists(schemaName);
        }

        internal static void DropTable(this MySqlConnection connection, string schemaName, string tableNme)
        {
            new MySqlSchemaExplorer(connection).DropTable(schemaName, tableNme);
        }
    }
}
