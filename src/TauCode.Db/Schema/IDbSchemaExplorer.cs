using System.Collections.Generic;
using TauCode.Db.Model;

namespace TauCode.Db.Schema
{
    public interface IDbSchemaExplorer
    {
        IReadOnlyList<string> GetTableNames(string schemaName);

        IReadOnlyList<string> GetTableNames(string schemaName, bool independentFirst);

        IReadOnlyList<ColumnMold> GetTableColumns(string schemaName, string tableName);

        PrimaryKeyMold GetTablePrimaryKey(string schemaName, string tableName);

        void ResolveIdentities(string schemaName, string tableName, IList<ColumnInfo2> columnInfos);

        IReadOnlyList<ForeignKeyMold> GetTableForeignKeys(string schemaName, string tableName, bool loadColumns);

        IReadOnlyList<IndexMold> GetTableIndexes(string schemaName, string tableName);

        TableMold GetTable(
            string schemaName,
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
    }
}
