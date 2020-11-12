using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db.Model;

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
            var schemaNames = schemaExplorer.GetSchemaNames();

            var forbiddenSchemaNames = new List<string>(schemaExplorer.GetSystemSchemaNames());
            if (schemaExplorer.DefaultSchemaName != null)
            {
                forbiddenSchemaNames.Add(schemaExplorer.DefaultSchemaName);
            }

            foreach (var schema in schemaNames)
            {
                var tableNames = schemaExplorer.GetTableNames(schema, false);

                foreach (var tableName in tableNames)
                {
                    schemaExplorer.DropTable(schema, tableName);
                }

                if (forbiddenSchemaNames.Contains(schema))
                {
                    continue;
                }

                schemaExplorer.DropSchema(schema);
            }
        }

        public static void CheckSchemaExistence(this IDbSchemaExplorer schemaExplorer, string schemaName)
        {
            if (!schemaExplorer.SchemaExists(schemaName))
            {
                throw DbTools.CreateSchemaDoesNotExistException(schemaName);
            }
        }

        public static void CheckSchemaAndTableExistence(
            this IDbSchemaExplorer schemaExplorer,
            string schemaName,
            string tableName)
        {
            // todo check args

            schemaExplorer.CheckSchemaExistence(schemaName);

            if (!schemaExplorer.TableExists(schemaName, tableName))
            {
                throw DbTools.CreateTableDoesNotExistException(schemaName, tableName);
            }
        }

        public static IList<TableMold> FilterTables(
            this IDbSchemaExplorer schemaExplorer,
            string schemaName,
            bool? independentFirst = null,
            Func<string, bool> tableNamePredicate = null)
        {
            if (schemaExplorer == null)
            {
                throw new ArgumentNullException(nameof(schemaExplorer));
            }

            tableNamePredicate ??= x => true;
            IEnumerable<string> tableNames;

            if (independentFirst.HasValue)
            {
                tableNames = schemaExplorer.GetTableNames(schemaName, independentFirst.Value);
            }
            else
            {
                tableNames = schemaExplorer.GetTableNames(schemaName);
            }

            var tables = tableNames
                .Where(x => tableNamePredicate(x))
                .Select(x => schemaExplorer.GetTable(
                    schemaName,
                    x,
                    true,
                    true,
                    true,
                    true))
                .ToList();

            return tables;
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
