using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db.Model;

namespace TauCode.Db.Schema
{
    // todo regions
    public abstract class DbSchemaExplorerBase : IDbSchemaExplorer
    {
        protected DbSchemaExplorerBase(IDbConnection connection)
        {
            this.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        protected IDbConnection Connection { get; }

        public virtual IReadOnlyList<string> GetTableNames(string schemaName)
        {
            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    T.table_name TableName
FROM
    information_schema.tables T
WHERE
    T.table_type = 'BASE TABLE' AND
    T.table_schema = @p_schemaName
ORDER BY
    T.table_name
";

            command.AddParameterWithValue("p_schemaName", schemaName);

            var tableNames = command
                .GetCommandRows()
                .Select(x => (string)x.TableName)
                .ToList();

            return tableNames;
        }

        public virtual IReadOnlyList<string> GetTableNames(string schemaName, bool independentFirst)
        {
            throw new NotImplementedException();
        }

        public virtual IReadOnlyList<ColumnMold> GetTableColumns(string schemaName, string tableName)
        {
            throw new NotImplementedException();
        }

        public virtual PrimaryKeyMold GetTablePrimaryKey(string schemaName, string tableName)
        {
            throw new NotImplementedException();
        }

        public virtual IReadOnlyList<ForeignKeyMold> GetTableForeignKeys(string schemaName, string tableName)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<IndexMold> GetTableIndexes(string schemaName, string tableName)
        {
            throw new NotImplementedException();
        }

        public virtual TableMold GetTable(
            string schemaName,
            bool includeColumns,
            bool includePrimaryKey,
            bool includeForeignKeys,
            bool includeIndexes)
        {
            throw new NotImplementedException();
        }

        public virtual IReadOnlyList<TableMold> GetTables(
            string schemaName,
            bool includeColumns,
            bool includePrimaryKey,
            bool includeForeignKeys,
            bool includeIndexes,
            bool? independentFirst)
        {
            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            if (independentFirst.HasValue && !includeForeignKeys)
            {
                throw new NotImplementedException(); // todo
            }

            var tableNames = this.GetTableNames(schemaName);

            var tables = new List<TableMold>();

            foreach (var tableName in tableNames)
            {
                var tableMold = new TableMold
                {
                    Name = tableName
                };

                if (includeColumns)
                {
                    var columns = this.GetTableColumns(schemaName, tableName);
                    tableMold.Columns = columns.ToList();
                }

                if (includePrimaryKey)
                {
                    tableMold.PrimaryKey = this.GetTablePrimaryKey(schemaName, tableName);
                }

                if (includeForeignKeys)
                {
                    var foreignKeys = this.GetTableForeignKeys(schemaName, tableName);
                    tableMold.ForeignKeys = foreignKeys.ToList();
                }

                if (includeIndexes)
                {
                    var indexes = this.GetTableIndexes(schemaName, tableName);
                    tableMold.Indexes = indexes.ToList();
                }

                tables.Add(tableMold);
            }

            if (independentFirst.HasValue)
            {
                tables = DbTools.ArrangeTables(tables, independentFirst.Value);
            }

            return tables;
        }
    }
}
