using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql.Tests
{
    // todo clean up
    internal static class TestHelper
    {
        internal const string ConnectionString = @"Server=localhost;Database=foo;Uid=root;Pwd=1234;";

        internal static MySqlConnection CreateConnection()
        {
            var connection = new MySqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        internal static void Purge(this MySqlConnection connection)
        {
            var schemata = connection.GetSchemata();

            foreach (var schema in schemata)
            {
                var tableNames = connection.GetTableNames(schema, false);
                foreach (var tableName in tableNames)
                {
                    connection.DropTable(schema, tableName);
                }

                if (schema == "foo")
                {
                    continue;
                }

                connection.DropSchema(schema);
            }

            if (!schemata.Contains("foo"))
            {
                connection.CreateSchema("foo");
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
            MySqlConnection connection,
            string schemaName,
            string tableName,
            object id)
        {
            IDbTableInspector tableInspector = new MySqlTableInspectorLab(connection, schemaName, tableName);
            var table = tableInspector.GetTable();
            var pkColumnName = table.GetPrimaryKeySingleColumn().Name;

            using var command = connection.CreateCommand();
            command.CommandText = $@"
SELECT
    *
FROM
    [{schemaName}].[{tableName}]
WHERE
    [{pkColumnName}] = @p_id
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

        internal static decimal GetLastIdentity(this MySqlConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT @@IDENTITY";
            return (decimal)command.ExecuteScalar();
        }

        internal static int GetTableRowCount(MySqlConnection connection, string schemaName, string tableName)
        {
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(*) FROM [{schemaName}].[{tableName}]";
            var count = (int)command.ExecuteScalar();
            return count;
        }

        internal static MySqlConnection CreateConnection(string schemaName)
        {
            var connectionString = $@"Server=localhost;Database={schemaName};Uid=root;Pwd=1234;";
            var connection = new MySqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
