using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient.Tests
{
    internal static class TestHelper
    {
        internal const string ConnectionString = @"Server=.\mssqltest;Database=rho.test;Trusted_Connection=True;";

        internal static SqlConnection CreateConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        internal static void Purge(this SqlConnection connection)
        {
            var schemata = connection.GetSchemata();

            foreach (var schema in schemata)
            {
                var tableNames = connection.GetTableNames(schema, false);
                foreach (var tableName in tableNames)
                {
                    connection.DropTable(schema, tableName);
                }

                if (schema != SqlToolsLab.DefaultSchemaName)
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
            SqlConnection connection,
            string schemaName,
            string tableName,
            object id)
        {
            IDbTableInspector tableInspector = new SqlTableInspectorLab(connection, schemaName, tableName);
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
            reader.Read();

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

        internal static decimal GetLastIdentity(this SqlConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT @@IDENTITY";
            return (decimal)command.ExecuteScalar();
        }
    }
}
