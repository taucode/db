using System.Collections.Generic;
using TauCode.Db.Schema;

namespace TauCode.Db.Extensions
{
    public static class DbSchemaExplorerExtensions
    {
        public static void DropAllTables(this IDbSchemaExplorer schemaExplorer, string schemaName)
        {
            var tableNames = schemaExplorer.GetTableNames(null, false);

            foreach (var tableName in tableNames)
            {
                schemaExplorer.DropTable(schemaName, tableName);
            }
        }

        public static void PurgeDatabase(this IDbSchemaExplorer schemaExplorer)
        {
            var schemata = schemaExplorer.GetSchemata();

            var forbiddenSchemata = new List<string>(schemaExplorer.GetSystemSchemata());
            if (schemaExplorer.DefaultSchemaName != null)
            {
                forbiddenSchemata.Add(schemaExplorer.DefaultSchemaName);
            }

            foreach (var schema in schemata)
            {
                var tableNames = schemaExplorer.GetTableNames(schema, false);

                foreach (var tableName in tableNames)
                {
                    schemaExplorer.DropTable(schema, tableName);
                }

                if (forbiddenSchemata.Contains(schema))
                {
                    continue;
                }

                schemaExplorer.DropSchema(schema);
            }
        }

        public static void CheckSchema(this IDbSchemaExplorer schemaExplorer, string schemaName)
        {
            if (!schemaExplorer.SchemaExists(schemaName))
            {
                throw DbTools.CreateSchemaDoesNotExistException(schemaName);
            }
        }

        public static void CheckSchemaAndTable(this IDbSchemaExplorer schemaExplorer, string schemaName, string tableName)
        {
            // todo check args

            schemaExplorer.CheckSchema(schemaName);

            if (!schemaExplorer.TableExists(schemaName, tableName))
            {
                throw DbTools.CreateTableDoesNotExistException(schemaName, tableName);
            }
        }

        public static void CreateSchema(this IDbSchemaExplorer schemaExplorer, string schemaName)
        {
            var sql = schemaExplorer.BuildCreateSchemaScript(schemaName);
            schemaExplorer.Connection.ExecuteSingleSql(sql);
        }

        public static void DropSchema(this IDbSchemaExplorer schemaExplorer, string schemaName)
        {
            var sql = schemaExplorer.BuildDropSchemaScript(schemaName);
            schemaExplorer.Connection.ExecuteSingleSql(sql);
        }

        public static void DropTable(this IDbSchemaExplorer schemaExplorer, string schemaName, string tableName)
        {
            var sql = schemaExplorer.BuildDropTableScript(schemaName, tableName);
            schemaExplorer.Connection.ExecuteSingleSql(sql);
        }
    }
}
