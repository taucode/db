using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TauCode.Db.Data;
using TauCode.Db.Model;

namespace TauCode.Db.Schema
{
    // todo regions
    // todo clean
    public abstract class DbSchemaExplorerBase : IDbSchemaExplorer
    {
        protected static readonly string[] StandardColumnTableColumnNames =
        {
            "column_name",
            "is_nullable",
            "data_type",
            "character_maximum_length",
            "numeric_precision",
            "numeric_scale",
        };

        protected DbSchemaExplorerBase(IDbConnection connection)
        {
            this.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        protected IDbConnection Connection { get; }

        protected virtual IList<string> GetAdditionalColumnTableColumnNames() => new string[0];

        protected virtual bool ParseBoolean(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value is bool b)
            {
                return b;
            }

            if (value is string s)
            {
                if (s.ToLower() == "yes")
                {
                    return true;
                }
                else if (s.ToLower() == "no")
                {
                    return false;
                }
                else
                {
                    throw new ArgumentException($"Could not parse value '{s}' as boolean.");
                }
            }

            throw new ArgumentException(
                $"Could not parse value '{value}' of type '{value.GetType().FullName}' as boolean.");
        }

        protected virtual int? GetDbValueAsInt(object dbValue)
        {
            if (dbValue == null)
            {
                return null;
            }

            var longValue = long.Parse(dbValue.ToString());
            if (longValue > int.MaxValue)
            {
                return null;
            }

            return (int) longValue;
        }

        protected virtual List<ColumnInfo2> GetColumnInfos(string schemaName, string tableName)
        {
            var sb = new StringBuilder();
            sb.AppendLine("SELECT");

            var allColumnNames = new List<string>(StandardColumnTableColumnNames);

            var additionalColumnNames = this.GetAdditionalColumnTableColumnNames();
            allColumnNames.AddRange(additionalColumnNames);

            for (var i = 0; i < allColumnNames.Count; i++)
            {
                var columnName = allColumnNames[i];
                sb.Append($"    C.{columnName}");
                if (i < allColumnNames.Count - 1)
                {
                    sb.Append(",");
                }

                sb.AppendLine();
            }

            sb.AppendLine(@"
FROM
    information_schema.columns C
WHERE
    C.table_name = @p_tableName AND
    C.table_schema = @p_schema
ORDER BY
    C.ordinal_position
");

            using var command = this.Connection.CreateCommand();
            command.CommandText = sb.ToString();

            command.AddParameterWithValue("p_schema", schemaName);
            command.AddParameterWithValue("p_tableName", tableName);

            var rows = command.GetCommandRows();

            var columnInfos = new List<ColumnInfo2>();

            foreach (var row in rows)
            {
                var columnInfo = new ColumnInfo2
                {
                    Name = row.column_name,
                    TypeName = row.data_type,
                    IsNullable = this.ParseBoolean(row.is_nullable),
                    Size = this.GetDbValueAsInt(row.character_maximum_length),
                    Precision = this.GetDbValueAsInt(row.numeric_precision),
                    Scale = this.GetDbValueAsInt(row.numeric_scale),
                };

                var dynamicRow = (DynamicRow) row;
                foreach (var additionalColumnName in additionalColumnNames)
                {
                    var additionalValue = dynamicRow.GetValue(additionalColumnName);
                    if (additionalValue == DBNull.Value)
                    {
                        continue;
                    }

                    columnInfo.Additional[additionalColumnName] = additionalValue.ToString();
                }

                columnInfos.Add(columnInfo);
            }

            return columnInfos;
        }

        protected abstract ColumnMold ColumnInfoToColumn(ColumnInfo2 columnInfo);

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
                .Select(x => (string) x.TableName)
                .ToList();

            return tableNames;
        }

        public virtual IReadOnlyList<string> GetTableNames(string schemaName, bool independentFirst)
        {
            throw new NotImplementedException();
        }

        public virtual IReadOnlyList<ColumnMold> GetTableColumns(string schemaName, string tableName)
        {
            var columnInfos = this.GetColumnInfos(schemaName, tableName);
            this.ResolveIdentities(schemaName, tableName, columnInfos);

            var columns = new List<ColumnMold>();

            foreach (var columnInfo in columnInfos)
            {
                var columnMold = this.ColumnInfoToColumn(columnInfo);
                if (columnInfo.Additional.ContainsKey("#identity_seed"))
                {
                    columnMold.Identity = new ColumnIdentityMold
                    {
                        Seed = columnInfo.Additional["#identity_seed"],
                        Increment = columnInfo.Additional.GetValueOrDefault("#identity_increment")
                    };
                }

                columns.Add(columnMold);
            }

            return columns;

            //return columnInfos
            //    .Select(this.ColumnInfoToColumn)
            //    .ToList();
        }

        public virtual PrimaryKeyMold GetTablePrimaryKey(string schemaName, string tableName)
        {
            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            using var command = this.Connection.CreateCommand();

            command.CommandText = @"
SELECT
    TC.constraint_name ConstraintName,
    KCU.column_name ColumnName
FROM
    information_schema.table_constraints TC
INNER JOIN
    information_schema.key_column_usage KCU
ON
    KCU.constraint_name = TC.constraint_name
WHERE
    TC.CONSTRAINT_SCHEMA = @p_schemaName AND
    TC.TABLE_SCHEMA = @p_schemaName AND
    TC.CONSTRAINT_TYPE = 'PRIMARY KEY' AND
    TC.TABLE_NAME = @p_tableName AND

    KCU.CONSTRAINT_SCHEMA = @p_schemaName AND
    KCU.TABLE_SCHEMA = @p_schemaName
ORDER BY
    KCU.ordinal_position
";

            command.AddParameterWithValue("p_schemaName", schemaName);
            command.AddParameterWithValue("p_tableName", tableName);

            var rows = command.GetCommandRows();

            if (rows.Count == 0)
            {
                return null; // no PK
            }

            var constraintName = (string) rows[0].ConstraintName;
            var columnNames = rows
                .Select(x => (string) x.ColumnName)
                .ToList();

            var pk = new PrimaryKeyMold
            {
                Name = constraintName,
                Columns = columnNames,
            };

            return pk;
        }

        public abstract void ResolveIdentities(string schemaName, string tableName, IList<ColumnInfo2> columnInfos);

        public virtual IReadOnlyList<ForeignKeyMold> GetTableForeignKeys(
            string schemaName,
            string tableName,
            bool loadColumns)
        {
            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    TC.constraint_name ConstraintName,
    RC.unique_constraint_name UniqueConstraintName,
    TC2.table_name ReferencedTableName
FROM
    information_schema.table_constraints TC
INNER JOIN
    information_schema.referential_constraints RC
ON
    TC.constraint_name = RC.constraint_name
INNER JOIN
    information_schema.table_constraints TC2
ON
    TC2.constraint_name = RC.unique_constraint_name AND TC2.constraint_type = 'PRIMARY KEY'
WHERE
    TC.table_name = @p_tableName AND
    TC.constraint_type = 'FOREIGN KEY' AND

    TC.constraint_schema = @p_schemaName AND
    TC.table_schema = @p_schemaName AND

    RC.constraint_schema = @p_schemaName AND
    RC.unique_constraint_schema = @p_schemaName AND

    TC2.constraint_schema = @p_schemaName AND
    TC2.table_schema = @p_schemaName
";

            command.AddParameterWithValue("@p_schemaName", schemaName);
            command.AddParameterWithValue("@p_tableName", tableName);

            var foreignKeyMolds = command
                .GetCommandRows()
                .Select(x => new ForeignKeyMold
                {
                    Name = x.ConstraintName,
                    ReferencedTableName = x.ReferencedTableName,
                })
                .ToList();

            if (loadColumns)
            {
                command.CommandText = @"
SELECT
    CU.constraint_name  ConstraintName,
    CU.column_name      ColumnName,
    CU2.column_name     ReferencedColumnName
FROM
    information_schema.key_column_usage CU
INNER JOIN
    information_schema.referential_constraints RC
ON
    CU.constraint_name = RC.constraint_name
INNER JOIN
    information_schema.key_column_usage CU2
ON
    RC.unique_constraint_name = CU2.constraint_name AND
    CU.ordinal_position = CU2.ordinal_position
WHERE
    CU.constraint_name = @p_fkName AND
    CU.constraint_schema = @p_schemaName AND
    CU.table_schema = @p_schemaName AND

    CU2.CONSTRAINT_SCHEMA = @p_schemaName AND
    CU2.TABLE_SCHEMA = @p_schemaName
ORDER BY
    CU.ordinal_position
";

                command.Parameters.Clear();

                //var fkParam = command.Parameters.Add("p_fkName", SqlDbType.NVarChar, 100);
                //var schemaParam = command.Parameters.Add("p_schemaName", SqlDbType.NVarChar, 100);

                var fkParam = command.CreateParameter();
                fkParam.ParameterName = "p_fkName";
                fkParam.DbType = DbType.String;
                fkParam.Size = 100;
                command.Parameters.Add(fkParam);

                var schemaParam = command.CreateParameter();
                schemaParam.ParameterName = "p_schemaName";
                schemaParam.DbType = DbType.String;
                schemaParam.Size = 100;
                command.Parameters.Add(schemaParam);

                schemaParam.Value = schemaName;

                command.Prepare();

                foreach (var fk in foreignKeyMolds)
                {
                    fkParam.Value = fk.Name;

                    var rows = command.GetCommandRows();

                    fk.ColumnNames = rows
                        .Select(x => (string)x.ColumnName)
                        .ToList();

                    fk.ReferencedColumnNames = rows
                        .Select(x => (string)x.ReferencedColumnName)
                        .ToList();
                }
            }

            return foreignKeyMolds;
        }

        public abstract IReadOnlyList<IndexMold> GetTableIndexes(string schemaName, string tableName);

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
                    var foreignKeys = this.GetTableForeignKeys(schemaName, tableName, true);
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
