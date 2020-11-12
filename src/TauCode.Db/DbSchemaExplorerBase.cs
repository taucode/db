using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TauCode.Db.Data;
using TauCode.Db.Extensions;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public abstract class DbSchemaExplorerBase : IDbSchemaExplorer
    {
        #region Constants

        protected static readonly string[] StandardColumnTableColumnNames =
        {
            "column_name",
            "is_nullable",
            "data_type",
            "character_maximum_length",
            "numeric_precision",
            "numeric_scale",
        };

        #endregion

        #region Constructor

        protected DbSchemaExplorerBase(IDbConnection connection, string delimiters)
        {
            this.Connection = connection ?? throw new ArgumentNullException(nameof(connection));

            if (delimiters == null)
            {
                this.OpeningDelimiter = null;
                this.ClosingDelimiter = null;
            }
            else
            {
                if (delimiters.Length != 2)
                {
                    throw new ArgumentException($"'{nameof(delimiters)}' should contain exactly 2 chars.",
                        nameof(delimiters));
                }

                this.OpeningDelimiter = delimiters[0];
                this.ClosingDelimiter = delimiters[1];
            }
        }

        #endregion

        #region Protected

        protected char? OpeningDelimiter { get; }
        protected char? ClosingDelimiter { get; }

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

            return (int)longValue;
        }

        protected virtual List<ColumnInfo> GetColumnInfos(string schemaName, string tableName)
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

            var columnInfos = new List<ColumnInfo>();

            foreach (var row in rows)
            {
                var columnInfo = new ColumnInfo
                {
                    Name = row.column_name,
                    TypeName = row.data_type,
                    IsNullable = this.ParseBoolean(row.is_nullable),
                    Size = this.GetDbValueAsInt(row.character_maximum_length),
                    Precision = this.GetDbValueAsInt(row.numeric_precision),
                    Scale = this.GetDbValueAsInt(row.numeric_scale),
                };

                var dynamicRow = (DynamicRow)row;
                foreach (var additionalColumnName in additionalColumnNames)
                {
                    var additionalValue = dynamicRow.GetValue(additionalColumnName);
                    if (additionalValue == null)
                    {
                        continue;
                    }

                    columnInfo.Additional[additionalColumnName] = additionalValue.ToString();
                }

                columnInfos.Add(columnInfo);
            }

            return columnInfos;
        }

        protected abstract ColumnMold ColumnInfoToColumn(ColumnInfo columnInfo);

        protected abstract IReadOnlyList<IndexMold> GetTableIndexesImpl(string schemaName, string tableName);

        protected abstract void ResolveIdentities(
            string schemaName,
            string tableName,
            IList<ColumnInfo> columnInfos);

        #endregion

        #region IDbSchemaExplorer Members

        public IDbConnection Connection { get; }

        public abstract IReadOnlyList<string> GetSystemSchemata();

        public abstract string DefaultSchemaName { get; }

        public virtual IReadOnlyList<string> GetSchemata()
        {
            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    S.schema_name SchemaName
FROM
    information_schema.schemata S
";

            var schemata = command
                .GetCommandRows()
                .Select(x => (string)x.SchemaName)
                .Except(this.GetSystemSchemata())
                .ToList();

            return schemata;
        }

        public virtual bool SchemaExists(string schemaName)
        {
            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    S.schema_name SchemaName
FROM
    information_schema.schemata S
WHERE
    S.schema_name = @p_schemaName
";
            command.AddParameterWithValue("p_schemaName", schemaName);

            var schemata = command
                .GetCommandRows()
                .Select(x => (string)x.SchemaName)
                .Except(this.GetSystemSchemata())
                .ToList();

            return schemata.Count == 1;
        }

        public virtual bool TableExists(string schemaName, string tableName)
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
    T.table_name
FROM
    information_schema.tables T
WHERE
    T.table_schema = @p_schemaName AND
    T.table_name = @p_tableName
";
            command.AddParameterWithValue("p_schemaName", schemaName);
            command.AddParameterWithValue("p_tableName", tableName);

            using var reader = command.ExecuteReader();
            return reader.Read();
        }

        public virtual IReadOnlyList<string> GetTableNames(string schemaName)
        {
            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            this.CheckSchemaExistence(schemaName);

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
            var tables = this.GetTables(
                schemaName,
                false,
                false,
                true,
                false,
                independentFirst);

            return tables.Select(x => x.Name).ToList();
        }

        public virtual IReadOnlyList<ColumnMold> GetTableColumns(
            string schemaName,
            string tableName,
            bool checkExistence)
        {
            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (checkExistence)
            {
                this.CheckSchemaAndTableExistence(schemaName, tableName);
            }

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
        }

        public virtual PrimaryKeyMold GetTablePrimaryKey(string schemaName, string tableName, bool checkExistence)
        {
            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (checkExistence)
            {
                this.CheckSchemaAndTableExistence(schemaName, tableName);
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
    TC.constraint_schema = @p_schemaName AND
    TC.table_schema = @p_schemaName AND
    TC.constraint_type = 'PRIMARY KEY' AND
    TC.table_name = @p_tableName AND

    KCU.constraint_schema = @p_schemaName AND
    KCU.table_schema = @p_schemaName
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

            var constraintName = (string)rows[0].ConstraintName;
            var columnNames = rows
                .Select(x => (string)x.ColumnName)
                .ToList();

            var pk = new PrimaryKeyMold
            {
                Name = constraintName,
                Columns = columnNames,
            };

            return pk;
        }

        public virtual IReadOnlyList<ForeignKeyMold> GetTableForeignKeys(
            string schemaName,
            string tableName,
            bool loadColumns,
            bool checkExistence)
        {
            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (checkExistence)
            {
                this.CheckSchemaAndTableExistence(schemaName, tableName);
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

        public virtual IReadOnlyList<IndexMold> GetTableIndexes(
            string schemaName,
            string tableName,
            bool checkExistence)
        {
            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (checkExistence)
            {
                this.CheckSchemaAndTableExistence(schemaName, tableName);
            }

            return this.GetTableIndexesImpl(schemaName, tableName);
        }

        public virtual TableMold GetTable(
            string schemaName,
            string tableName,
            bool includeColumns,
            bool includePrimaryKey,
            bool includeForeignKeys,
            bool includeIndexes)
        {
            // todo checks

            this.CheckSchemaAndTableExistence(schemaName, tableName);

            var tableMold = new TableMold
            {
                Name = tableName,
            };

            if (includeColumns)
            {
                var columns = this.GetTableColumns(schemaName, tableName, false);
                tableMold.Columns = columns.ToList();
            }

            if (includePrimaryKey)
            {
                tableMold.PrimaryKey = this.GetTablePrimaryKey(schemaName, tableName, false);
            }

            if (includeForeignKeys)
            {
                var foreignKeys = this.GetTableForeignKeys(schemaName, tableName, true, false);
                tableMold.ForeignKeys = foreignKeys.ToList();
            }

            if (includeIndexes)
            {
                var indexes = this.GetTableIndexes(schemaName, tableName, false);
                tableMold.Indexes = indexes.ToList();
            }

            return tableMold;
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
                throw new ArgumentException($"If '{nameof(independentFirst)}' value is provided, '{nameof(includeForeignKeys)}' must be true.");
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
                    var columns = this.GetTableColumns(schemaName, tableName, false);
                    tableMold.Columns = columns.ToList();
                }

                if (includePrimaryKey)
                {
                    tableMold.PrimaryKey = this.GetTablePrimaryKey(schemaName, tableName, false);
                }

                if (includeForeignKeys)
                {
                    var foreignKeys = this.GetTableForeignKeys(schemaName, tableName, true, false);
                    tableMold.ForeignKeys = foreignKeys.ToList();
                }

                if (includeIndexes)
                {
                    var indexes = this.GetTableIndexes(schemaName, tableName, false);
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

        public string BuildCreateSchemaScript(string schemaName)
        {

            var sb = new StringBuilder();

            sb.AppendLine("CREATE SCHEMA ");
            if (schemaName != null)
            {
                sb.Append($"{this.OpeningDelimiter}{schemaName}{this.ClosingDelimiter}");
            }

            return sb.ToString();
        }

        public string BuildDropSchemaScript(string schemaName)
        {
            var sb = new StringBuilder();

            sb.AppendLine("DROP SCHEMA ");
            if (schemaName != null)
            {
                sb.Append($"{this.OpeningDelimiter}{schemaName}{this.ClosingDelimiter}");
            }

            return sb.ToString();
        }

        public string BuildDropTableScript(string schemaName, string tableName)
        {
            var sb = new StringBuilder();

            sb.AppendLine("DROP TABLE ");
            if (schemaName != null)
            {
                sb.Append($"{this.OpeningDelimiter}{schemaName}{this.ClosingDelimiter}");
                sb.Append(".");
            }

            sb.Append($"{this.OpeningDelimiter}{tableName}{this.ClosingDelimiter}");

            return sb.ToString();
        }

        #endregion
    }
}