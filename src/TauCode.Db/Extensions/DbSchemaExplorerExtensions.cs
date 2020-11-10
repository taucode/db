using System;
using TauCode.Db.Schema;

namespace TauCode.Db.Extensions
{
    public static class DbSchemaExplorerExtensions
    {
        public static void PurgeDatabase(this IDbSchemaExplorer schemaExplorer)
        {
            throw new NotImplementedException();
        }

        public static void CheckSchema(this IDbSchemaExplorer schemaExplorer, string schemaName)
        {
            // todo check args

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
    }
}
