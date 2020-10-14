using Microsoft.Data.SqlClient;

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
    }
}
