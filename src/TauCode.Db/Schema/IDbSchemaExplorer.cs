using System.Collections.Generic;
using System.Data;
using TauCode.Db.Model;

namespace TauCode.Db.Schema
{
    public interface IDbSchemaExplorer
    {
        IDbConnection Connection { get; }

        IReadOnlyList<string> GetSystemSchemata();

        string DefaultSchemaName { get; }

        IReadOnlyList<string> GetSchemata();

        bool SchemaExists(string schemaName);

        bool TableExists(string schemaName, string tableName);

        IReadOnlyList<string> GetTableNames(string schemaName);

        IReadOnlyList<string> GetTableNames(string schemaName, bool independentFirst);

        IReadOnlyList<ColumnMold> GetTableColumns(string schemaName, string tableName, bool checkExistence);

        PrimaryKeyMold GetTablePrimaryKey(string schemaName, string tableName, bool checkExistence);

        IReadOnlyList<ForeignKeyMold> GetTableForeignKeys(string schemaName, string tableName, bool loadColumns, bool checkExistence);

        IReadOnlyList<IndexMold> GetTableIndexes(string schemaName, string tableName, bool checkExistence);

        TableMold GetTable(
            string schemaName,
            string tableName,
            bool includeColumns,
            bool includePrimaryKey,
            bool includeForeignKeys,
            bool includeIndexes);

        IReadOnlyList<TableMold> GetTables(
            string schemaName,
            bool includeColumns,
            bool includePrimaryKey,
            bool includeForeignKeys,
            bool includeIndexes,
            bool? independentFirst);

        string BuildCreateSchemaScript(string schemaName);

        string BuildDropSchemaScript(string schemaName);

        string BuildDropTableScript(string schemaName, string tableName);
    }
}
