using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;
using TauCode.Db.Utils.Dialects;
using TauCode.Db.Utils.Inspection;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Utils.Building
{
    public abstract class ScriptBuilderBase : IScriptBuilder
    {
        #region Fields

        private char? _currentOpeningIdentifierDelimiter;
        private char? _currentClosingIdentifierDelimiter;

        #endregion

        #region Constructor

        protected ScriptBuilderBase(IDialect dialect)
        {
            this.Dialect = dialect ?? throw new ArgumentNullException(nameof(dialect));
        }

        #endregion

        #region Polymorph

        protected abstract IDbInspector CreateDbInspector(IDbConnection connection);

        protected abstract string BuildIdentitySubSql(ColumnIdentityMold identity);

        protected virtual string BuildColumnPropertiesSubSql(ColumnMold column)
        {
            return string.Empty;
        }

        protected abstract void AddIndexCreationIntoTableCreationScript(TableMold table, StringBuilder sb);

        protected abstract string BuildCreateTableSqlForCreateDbSql(TableMold table);

        protected virtual string BuildAlterTableSqlForCreateDbSql(TableMold table, bool addComments)
        {
            var neededIndexes = table.Indexes
                .Where(x => x.Name != table.PrimaryKey.Name)
                .OrderBy(x => x.Name)
                .ToArray();

            var sb = new StringBuilder();

            // indexes
            if (neededIndexes.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine();
            }

            for (var i = 0; i < neededIndexes.Length; i++)
            {
                var index = neededIndexes[i];

                if (addComments)
                {
                    var uniqueWord = index.IsUnique ? "unique " : string.Empty;
                    sb.AppendLine($@"/* create {uniqueWord}index {index.Name}: {table.Name}({string.Join(", ", index.Columns.Select(x => x.Name))}) */");
                }

                var indexSql = this.BuildIndexSql(table.Name, index);
                sb.Append(indexSql);

                if (i < neededIndexes.Length - 1)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                }
            }

            // foreign keys
            if (table.ForeignKeys.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine();
            }

            var foreignKeys = table.ForeignKeys
                .OrderBy(x => x.Name)
                .ToList();

            for (var i = 0; i < foreignKeys.Count; i++)
            {
                var foreignKey = foreignKeys[i];

                if (addComments)
                {
                    var from = $"{table.Name}({string.Join(", ", foreignKey.ColumnNames)})";
                    var to = $"{foreignKey.ReferencedTableName}({string.Join(", ", foreignKey.ReferencedColumnNames)})";
                    sb.AppendLine($@"/* create foreign key {foreignKey.Name}: {from} -> {to} */");
                }

                var foreignKeySql = this.BuildForeignKeySql(table.Name, foreignKey);
                sb.Append(foreignKeySql);

                if (i < foreignKeys.Count - 1)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        protected virtual string BuildTypeDefinitionSql(DbTypeMold type)
        {
            var decoratedTypeName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Type, type.Name,
                this.CurrentOpeningIdentifierDelimiter);

            var sb = new StringBuilder(decoratedTypeName);

            if (type.Size.HasValue)
            {
                if (type.Precision.HasValue || type.Scale.HasValue)
                {
                    throw new ScriptBuildingException("If type has Size, it must not have Precision and Scale.");
                }

                sb.Append($"({type.Size.Value})");
            }
            else if (type.Precision.HasValue)
            {
                if (type.Size.HasValue)
                {
                    throw new ScriptBuildingException("If type has Precision, it must not have Size.");
                }

                sb.Append($"({type.Precision.Value}");

                if (type.Scale.HasValue)
                {
                    sb.Append($", {type.Scale.Value}");
                }

                sb.Append(")");
            }

            return sb.ToString();
        }

        protected virtual string BuildSelectSqlImpl(string tableName, string[] columnNames, bool addClauseTerminator)
        {
            var sb = new StringBuilder();
            sb.AppendLine("SELECT");
            var alias = "T";

            for (var i = 0; i < columnNames.Length; i++)
            {
                var columnName = columnNames[i];

                var decoratedColumnName = this.Dialect.DecorateIdentifier(
                    DbIdentifierType.Column,
                    columnName,
                    this.CurrentOpeningIdentifierDelimiter);

                sb.AppendFormat($"    {alias}.{decoratedColumnName}");

                if (i < columnNames.Length - 1)
                {
                    sb.Append(",");
                }

                sb.AppendLine();
            }

            sb.AppendLine("FROM");

            var decorateTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                tableName,
                this.CurrentOpeningIdentifierDelimiter);

            sb.Append($"    {decorateTableName} {alias}");

            if (addClauseTerminator)
            {
                sb.Append(this.EffectiveClauseTerminator);
            }

            return sb.ToString();
        }

        protected virtual string BuildDeleteSqlImpl(string tableName, bool addClauseTerminator)
        {
            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                tableName,
                this.CurrentOpeningIdentifierDelimiter);

            var ending = addClauseTerminator ? this.EffectiveClauseTerminator : "";

            return $"DELETE FROM {decoratedTableName}{ending}";
        }

        #endregion

        #region Protected

        protected string EffectiveClauseTerminator
        {
            get
            {
                if (this.AddClauseTerminator || this.Dialect.IsClauseTerminatorMandatory)
                {
                    return this.Dialect.ClauseTerminator;
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        #endregion

        #region IScriptBuilder Members

        public IDialect Dialect { get; }

        public char? CurrentOpeningIdentifierDelimiter
        {
            get => _currentOpeningIdentifierDelimiter;
            set
            {
                if (value.HasValue)
                {
                    var tuple = this.Dialect.IdentifierDelimiters.SingleOrDefault(x => x.Item1 == value.Value);
                    if (tuple == null)
                    {
                        throw new ArgumentException($"Unknown opening identifier delimiter: {value}", nameof(value));
                    }

                    _currentOpeningIdentifierDelimiter = value;
                    _currentClosingIdentifierDelimiter = tuple.Item2;
                }
                else
                {
                    _currentOpeningIdentifierDelimiter = null;
                    _currentClosingIdentifierDelimiter = null;
                }
            }
        }

        public bool AddClauseTerminator { get; set; }

        public virtual string BuildCreateColumnSql(ColumnMold column)
        {
            var sb = new StringBuilder();

            // name
            var decoratedColumnName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Column,
                column.Name,
                this.CurrentOpeningIdentifierDelimiter);

            sb.Append(decoratedColumnName);

            // type definition
            sb.Append(" ");

            var decoratedTypeDefinition = this.BuildTypeDefinitionSql(column.Type);
            sb.Append(decoratedTypeDefinition);

            // properties
            var columnPropertiesSql = this.BuildColumnPropertiesSubSql(column);
            if (!string.IsNullOrEmpty(columnPropertiesSql))
            {
                sb.Append(" ");
                sb.Append(columnPropertiesSql);
            }

            // nullability
            sb.Append(" ");

            var nullability = column.IsNullable ? "NULL" : "NOT NULL";
            sb.Append(nullability);

            // force inline primary key
            if (column.Properties.GetOrDefault("force-inline-primary-key")?.ToLower() == "true")
            {
                sb.Append(" PRIMARY KEY");
            }

            if (column.Identity != null)
            {
                sb.Append(" ");
                var identitySubSql = this.BuildIdentitySubSql(column.Identity);
                sb.Append(identitySubSql);
            }

            return sb.ToString();
        }

        public virtual string BuildPrimaryKeySql(string tableName, PrimaryKeyMold primaryKey)
        {
            var sb = new StringBuilder();
            if (tableName != null)
            {
                var decoratedTableName = this.Dialect.DecorateIdentifier(
                    DbIdentifierType.Table,
                    tableName,
                    this.CurrentOpeningIdentifierDelimiter);
                sb.AppendFormat("ALTER TABLE {0} ADD ", decoratedTableName);
            }

            var decoratedPrimaryKeyName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Constraint,
                primaryKey.Name,
                this.CurrentOpeningIdentifierDelimiter);

            var decoratedColumnNames = this.DecorateIndexColumnsOverComma(
                primaryKey.Columns,
                this.CurrentOpeningIdentifierDelimiter);

            sb.AppendFormat("CONSTRAINT {0} PRIMARY KEY({1})", decoratedPrimaryKeyName, decoratedColumnNames);

            return sb.ToString();
        }

        public virtual string BuildForeignKeySql(string tableName, ForeignKeyMold foreignKey)
        {
            var sb = new StringBuilder();
            if (tableName != null)
            {
                var decoratedTableName = this.Dialect.DecorateIdentifier(
                    DbIdentifierType.Table,
                    tableName,
                    this.CurrentOpeningIdentifierDelimiter);
                sb.AppendFormat("ALTER TABLE {0} ADD ", decoratedTableName);
            }

            if (foreignKey.Name != null)
            {
                var decoratedConstraintName = this.Dialect.DecorateIdentifier(
                    DbIdentifierType.Constraint,
                    foreignKey.Name,
                    this.CurrentOpeningIdentifierDelimiter);

                sb.AppendFormat("CONSTRAINT {0} ", decoratedConstraintName);
            }

            var decoratedColumnNames = this.DecorateColumnsOverComma(
                foreignKey.ColumnNames,
                this.CurrentOpeningIdentifierDelimiter);

            sb.AppendFormat("FOREIGN KEY({0})", decoratedColumnNames);

            var separator = tableName == null ? " " : Environment.NewLine;

            sb.Append(separator);

            var decoratedReferencedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                foreignKey.ReferencedTableName,
                this.CurrentOpeningIdentifierDelimiter);

            var decoratedReferencedColumnNames = this.DecorateColumnsOverComma(
                foreignKey.ReferencedColumnNames,
                this.CurrentOpeningIdentifierDelimiter);

            sb.AppendFormat("REFERENCES {0}({1})", decoratedReferencedTableName, decoratedReferencedColumnNames);

            return sb.ToString();
        }

        public virtual string BuildIndexSql(string tableName, IndexMold index)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var sb = new StringBuilder();

            sb.Append("CREATE ");

            if (index.IsUnique)
            {
                sb.Append("UNIQUE ");
            }

            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                tableName,
                this.CurrentOpeningIdentifierDelimiter);

            var decoratedIndexName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Index,
                index.Name,
                this.CurrentOpeningIdentifierDelimiter);

            var indexColumnsSql = this.DecorateIndexColumnsOverComma(
                index.Columns,
                this.CurrentOpeningIdentifierDelimiter);

            sb.AppendFormat(
                "INDEX {0} ON {1}({2})",
                decoratedIndexName,
                decoratedTableName,
                indexColumnsSql);

            return sb.ToString();
        }

        public virtual string BuildCreateTableSql(TableMold table, bool inline)
        {
            var sb = new StringBuilder();

            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                table.Name,
                this.CurrentOpeningIdentifierDelimiter);

            sb.AppendLine($@"CREATE TABLE {decoratedTableName}(");
            for (var i = 0; i < table.Columns.Count; i++)
            {
                var column = table.Columns[i];

                var columnSql = this.BuildCreateColumnSql(column);
                sb.AppendFormat("    {0}", columnSql);

                if (i < table.Columns.Count - 1)
                {
                    sb.AppendLine(",");
                }
            }

            // primary key
            if (table.PrimaryKey != null && !table.Columns.Any(x =>
                    x.Properties.GetOrDefault("force-inline-primary-key")?.ToLower() == "true"))
            {
                sb.AppendLine(",");

                var primaryKeySql = this.BuildPrimaryKeySql(null, table.PrimaryKey);
                sb.AppendFormat("    {0}", primaryKeySql);
            }

            if (inline)
            {
                // indexes
                this.AddIndexCreationIntoTableCreationScript(table, sb);

                // foreign keys
                if (table.ForeignKeys.Count > 0)
                {
                    sb.AppendLine(",");

                    for (var i = 0; i < table.ForeignKeys.Count; i++)
                    {
                        var foreignKey = table.ForeignKeys[i];
                        var foreignKeySql = this.BuildForeignKeySql(null, foreignKey);
                        sb.Append($"    {foreignKeySql}");

                        if (i < table.ForeignKeys.Count - 1)
                        {
                            sb.AppendLine(",");
                        }
                    }
                }
            }

            sb.Append(")");
            sb.Append(this.EffectiveClauseTerminator);

            return sb.ToString();
        }

        public virtual string BuildCreateDbSql(IDbConnection connection, bool addComments)
        {
            var dbInspector = this.CreateDbInspector(connection);
            var tables = dbInspector.GetOrderedTableMolds(true);

            var sb = new StringBuilder();

            for (var i = 0; i < tables.Length; i++)
            {
                var table = tables[i];

                if (addComments)
                {
                    sb.AppendLine($@"/* create table: {table.Name} */");
                }

                var createTableSql = this.BuildCreateTableSqlForCreateDbSql(table);
                sb.Append(createTableSql);

                var alterTableSql = this.BuildAlterTableSqlForCreateDbSql(table, addComments);
                sb.Append(alterTableSql);

                if (i < tables.Length - 1)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        public string BuildSelectSql(string tableName, string[] columnNames)
        {
            return this.BuildSelectSqlImpl(tableName, columnNames, true);
        }

        public virtual string BuildSelectSql(TableMold table) =>
            this.BuildSelectSql(
                table.Name,
                table.Columns
                    .Select(x => x.Name)
                    .ToArray());

        public string BuildSelectRowByIdSql(TableMold tableMold, out string idParamName)
        {
            var idColumnName = tableMold.GetSinglePrimaryKeyColumnName();
            var decoratedIdColumnName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Column,
                idColumnName,
                this.CurrentOpeningIdentifierDelimiter);

            idParamName = $"@_{idColumnName}";

            var sb = new StringBuilder();
            sb.Append(
                this.BuildSelectSqlImpl(
                    tableMold.Name,
                    tableMold.Columns
                        .Select(x => x.Name)
                        .ToArray(),
                    false));

            sb.AppendLine();
            sb.AppendLine("WHERE");
            sb.Append($"    T.{decoratedIdColumnName} = {idParamName}");

            return sb.ToString();
        }

        public virtual string BuildInsertSql(
            TableMold table,
            IDictionary<string, object> columnValues,
            int? indent = null)
        {
            var newLine = indent.HasValue ? Environment.NewLine : string.Empty;
            var spaces = indent.HasValue ? string.Empty.PadLeft(indent.Value) : string.Empty;
            var margin = indent.HasValue ? Environment.NewLine : " ";

            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            if (columnValues == null)
            {
                throw new ArgumentNullException(nameof(columnValues));
            }

            if (columnValues.Count == 0)
            {
                throw new NotSupportedException(
                    "Inserting all default values is not supported. Provide at least one column value.");
            }

            if (columnValues.Count > table.Columns.Count)
            {
                throw new ArgumentException("Too many column values provided.");
            }

            var columnValuesWithLowerCase = columnValues
                .ToDictionary(x => x.Key.ToLowerInvariant(), x => x.Value);

            var sb = new StringBuilder();
            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                table.Name,
                this.CurrentOpeningIdentifierDelimiter);

            sb.Append($"INSERT INTO {decoratedTableName}(");
            sb.Append(newLine);

            var columnNamesLowerCase = new List<string>();

            var entries = columnValues.ToArray();

            for (var i = 0; i < entries.Length; i++)
            {
                var entry = entries[i];
                var name = entry.Key;
                var column = table.Columns.SingleOrDefault(x =>
                    string.Equals(x.Name, name, StringComparison.InvariantCultureIgnoreCase));

                if (column == null)
                {
                    throw new ArgumentException(
                        $"Value with name '{name}' provided, but the respective column was not found in the table.");
                }

                sb.Append(spaces);

                var decoratedColumnName = this.Dialect.DecorateIdentifier(
                    DbIdentifierType.Column,
                    column.Name,
                    this.CurrentOpeningIdentifierDelimiter);

                columnNamesLowerCase.Add(column.Name.ToLowerInvariant());

                sb.Append(decoratedColumnName);

                if (i < entries.Length - 1)
                {
                    sb.Append(",");
                    sb.Append(margin);
                }
            }

            sb.Append($"){margin}VALUES(");
            sb.Append(newLine);

            for (var i = 0; i < columnNamesLowerCase.Count; i++)
            {
                var name = columnNamesLowerCase[i];

                var value = columnValuesWithLowerCase.GetOrDefault(name);
                var column = table.GetColumn(name);

                var valueString = this.Dialect.ValueToSqlValueString(column.Type, value);

                sb.Append(spaces);
                sb.Append(valueString);

                if (i < columnNamesLowerCase.Count - 1)
                {
                    sb.Append(",");
                    sb.Append(margin);
                }
            }

            sb.Append(")");

            sb.Append(this.EffectiveClauseTerminator);

            return sb.ToString();
        }

        public virtual string BuildParameterizedInsertSql(
            TableMold table,
            out IDictionary<string, string> parameterMapping,
            string[] columnsToInclude = null,
            string[] columnsToExclude = null,
            int? indent = null)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            if (columnsToInclude != null && columnsToExclude != null)
            {
                throw new ArgumentException(
                    $"Both '{nameof(columnsToInclude)}' and '{nameof(columnsToExclude)}' cannot be provided.");
            }

            var wantedColumnNames = new List<string>();

            if (columnsToInclude != null)
            {
                foreach (var columnName in columnsToInclude)
                {
                    var tableColumn = table.Columns.SingleOrDefault(x =>
                        string.Equals(x.Name, columnName, StringComparison.InvariantCultureIgnoreCase));

                    if (tableColumn == null)
                    {
                        throw new ArgumentException(
                            $"'{nameof(columnsToInclude)}' contains '{columnName}', but table doesn't have it.");
                    }

                    wantedColumnNames.Add(tableColumn.Name);
                }
            }

            if (columnsToExclude != null)
            {
                var copiedColumns = table.Columns.ToList();

                foreach (var columnName in columnsToExclude)
                {
                    var tableColumn = copiedColumns.SingleOrDefault(x =>
                        string.Equals(x.Name, columnName, StringComparison.InvariantCultureIgnoreCase));

                    if (tableColumn == null)
                    {
                        throw new ArgumentException(
                            $"'{nameof(columnsToExclude)}' contains '{columnName}', but table doesn't have it.");
                    }

                    copiedColumns.Remove(tableColumn);
                }

                wantedColumnNames = copiedColumns
                    .Select(x => x.Name)
                    .ToList();
            }

            parameterMapping = new Dictionary<string, string>();

            var newLine = indent.HasValue ? Environment.NewLine : string.Empty;
            var spaces = indent.HasValue ? string.Empty.PadLeft(indent.Value) : string.Empty;
            var margin = indent.HasValue ? Environment.NewLine : " ";

            var sb = new StringBuilder();
            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                table.Name,
                this.CurrentOpeningIdentifierDelimiter);

            sb.Append($"INSERT INTO {decoratedTableName}(");
            sb.Append(newLine);

            for (var i = 0; i < wantedColumnNames.Count; i++)
            {
                var columnName = wantedColumnNames[i];

                sb.Append(spaces);

                var decoratedColumnName = this.Dialect.DecorateIdentifier(
                    DbIdentifierType.Column,
                    columnName,
                    this.CurrentOpeningIdentifierDelimiter);

                sb.Append(decoratedColumnName);

                if (i < wantedColumnNames.Count - 1)
                {
                    sb.Append(",");
                    sb.Append(margin);
                }

                var paramName = $"@_{columnName}";
                parameterMapping.Add(columnName, paramName);
            }

            sb.Append($"){margin}VALUES(");
            sb.Append(newLine);

            for (var i = 0; i < wantedColumnNames.Count; i++)
            {
                var columnName = wantedColumnNames[i];
                var paramName = parameterMapping[columnName];

                sb.Append(spaces);
                sb.Append(paramName);

                if (i < wantedColumnNames.Count - 1)
                {
                    sb.Append(",");
                    sb.Append(margin);
                }
            }

            sb.Append(")");

            sb.Append(this.EffectiveClauseTerminator);

            return sb.ToString();
        }

        public string BuildDeleteSql(string tableName)
        {
            return this.BuildDeleteSqlImpl(tableName, true);
        }

        public virtual string BuildDeleteRowByIdSql(string tableName, string primaryKeyColumnName, out string paramName)
        {
            paramName = $"@_{primaryKeyColumnName}";
            var sb = new StringBuilder();
            sb.AppendLine(this.BuildDeleteSqlImpl(tableName, false));

            var decoratedColumnName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Column,
                primaryKeyColumnName,
                this.CurrentOpeningIdentifierDelimiter);

            sb.Append($"WHERE {decoratedColumnName} = {paramName}");

            return sb.ToString();
        }

        public string BuildUpdateRowByIdSql(
            string tableName,
            string primaryKeyColumnName,
            string[] columnNames,
            out Dictionary<string, string> parameterMapping)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (primaryKeyColumnName == null)
            {
                throw new ArgumentNullException(nameof(primaryKeyColumnName));
            }

            if (columnNames == null)
            {
                throw new ArgumentNullException();
            }

            if (columnNames.Length == 0)
            {
                throw new ArgumentException("At least one column name should be provided.");
            }

            parameterMapping = new Dictionary<string, string>();

            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                tableName,
                this.CurrentOpeningIdentifierDelimiter);

            var sb = new StringBuilder();
            sb.AppendLine($"UPDATE {decoratedTableName} SET");

            for (var i = 0; i < columnNames.Length; i++)
            {
                var columnName = columnNames[i];
                var decoratedColumnName = this.Dialect.DecorateIdentifier(
                    DbIdentifierType.Column,
                    columnName,
                    this.CurrentOpeningIdentifierDelimiter);

                var paramName = $"@_{columnName}";

                var commaOrNothing = i == columnNames.Length - 1 ? "" : ",";

                sb.AppendLine($"    {decoratedColumnName} = {paramName}{commaOrNothing}");

                parameterMapping.Add(columnName, paramName);
            }

            var decoratedIdColumnName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Column,
                primaryKeyColumnName,
                this.CurrentOpeningIdentifierDelimiter);

            var idParamName = $"@_{primaryKeyColumnName}";

            sb.AppendLine("WHERE");
            sb.Append($"    {decoratedIdColumnName} = {idParamName}");

            parameterMapping.Add(primaryKeyColumnName, idParamName);

            return sb.ToString();
        }

        public virtual string BuildFillTableSql(IDbConnection connection, TableMold table)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            var sql = this.BuildSelectSql(table);

            var sb = new StringBuilder();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;

                // won't use cruder here, to avoid blunt circular ref (cruder uses script builder)
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var values = new Dictionary<string, object>();
                        for (var i = 0; i < reader.FieldCount; i++)
                        {
                            var name = reader.GetName(i);
                            var value = reader[i];
                            if (value == DBNull.Value)
                            {
                                value = null;
                            }

                            values.Add(name, value);
                        }

                        var insertSql = this.BuildInsertSql(table, values);
                        sb.AppendLine(insertSql);
                    }

                    return sb.ToString();
                }
            }
        }

        public virtual string BuildFillDbSql(
            IDbConnection connection,
            string[] tableNamesToInclude = null,
            string[] tableNamesToExclude = null,
            bool deleteExistingData = false)
        {
            if (tableNamesToInclude != null && tableNamesToExclude != null)
            {
                throw new ArgumentException(
                    "Provide either 'tableNamesToInclude', or 'tableNamesToExclude', or neither of them, but not both",
                    "tableNamesToInclude/tableNamesToExclude");
            }

            var tableNamesToIncludeLowerCase = new HashSet<string>();

            if (tableNamesToInclude != null)
            {
                if (tableNamesToInclude.Contains(null))
                {
                    throw new ArgumentException(
                        $"'tableNamesToInclude' must not contain nulls",
                        nameof(tableNamesToInclude));
                }

                tableNamesToIncludeLowerCase = new HashSet<string>(tableNamesToInclude.Select(x => x.ToLower()));
            }

            var tableNamesToExcludeLowerCase = new HashSet<string>();

            if (tableNamesToExclude != null)
            {
                if (tableNamesToExclude.Contains(null))
                {
                    throw new ArgumentException(
                        $"'tableNamesToExclude' must not contain nulls",
                        nameof(tableNamesToExclude));
                }

                tableNamesToExcludeLowerCase = new HashSet<string>(tableNamesToExclude.Select(x => x.ToLower()));
            }

            var dbInspector = this.CreateDbInspector(connection);
            var tables = dbInspector.GetOrderedTableMolds(true);

            if (tableNamesToInclude != null)
            {
                tables = tables
                    .Where(x => tableNamesToIncludeLowerCase.Contains(x.Name.ToLower()))
                    .ToArray();
            }

            if (tableNamesToExclude != null)
            {
                tables = tables
                    .Where(x => !tableNamesToExcludeLowerCase.Contains(x.Name.ToLower()))
                    .ToArray();
            }

            var desiredTableNames = new HashSet<string>(tables.Select(x => x.Name.ToLower()));

            var sb = new StringBuilder();

            // delete existing if needed
            if (deleteExistingData)
            {
                var orderedTables = dbInspector.GetOrderedTableMolds(false)
                    .Where(x => desiredTableNames.Contains(x.Name.ToLower()))
                    .ToArray();

                foreach (var table in orderedTables)
                {
                    var deleteSql = this.BuildDeleteSql(table.Name);
                    sb.AppendLine($"/* delete from {table.Name} */");
                    sb.AppendLine(deleteSql);
                }

                if (orderedTables.Length > 0)
                {
                    sb.AppendLine();
                }
            }

            // insert data
            for (var i = 0; i < tables.Length; i++)
            {
                var table = tables[i];
                var tableName = table.Name.ToLower();

                sb.AppendLine($"/* {tableName} */");

                var fillTableSql = this.BuildFillTableSql(connection, table);
                sb.Append(fillTableSql);

                if (i < tables.Length - 1)
                {
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        public virtual string BuildClearDbSql(
            IDbConnection connection,
            string[] tableNamesToInclude = null,
            string[] tableNamesToExclude = null)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (tableNamesToInclude != null && tableNamesToExclude != null)
            {
                throw new ArgumentException(
                    "Provide either 'tableNamesToInclude', or 'tableNamesToExclude', or neither of them, but not both",
                    "tableNamesToInclude/tableNamesToExclude");
            }

            var tableNamesToIncludeLowerCase = new HashSet<string>();

            if (tableNamesToInclude != null)
            {
                if (tableNamesToInclude.Contains(null))
                {
                    throw new ArgumentException(
                        $"'tableNamesToInclude' must not contain nulls",
                        nameof(tableNamesToInclude));
                }

                tableNamesToIncludeLowerCase = new HashSet<string>(tableNamesToInclude.Select(x => x.ToLower()));
            }

            var tableNamesToExcludeLowerCase = new HashSet<string>();

            if (tableNamesToExclude != null)
            {
                if (tableNamesToExclude.Contains(null))
                {
                    throw new ArgumentException(
                        $"'tableNamesToExclude' must not contain nulls",
                        nameof(tableNamesToExclude));
                }

                tableNamesToExcludeLowerCase = new HashSet<string>(tableNamesToExclude.Select(x => x.ToLower()));
            }

            var dbInspector = this.CreateDbInspector(connection);
            var tables = dbInspector.GetOrderedTableMolds(true);

            if (tableNamesToInclude != null)
            {
                tables = tables
                    .Where(x => tableNamesToIncludeLowerCase.Contains(x.Name.ToLower()))
                    .ToArray();
            }

            if (tableNamesToExclude != null)
            {
                tables = tables
                    .Where(x => !tableNamesToExcludeLowerCase.Contains(x.Name.ToLower()))
                    .ToArray();
            }

            var desiredTableNames = new HashSet<string>(tables.Select(x => x.Name.ToLower()));

            var sb = new StringBuilder();

            var orderedTables = dbInspector.GetOrderedTableMolds(false)
                .Where(x => desiredTableNames.Contains(x.Name.ToLower()))
                .ToArray();

            foreach (var table in orderedTables)
            {
                var deleteSql = this.BuildDeleteSql(table.Name);
                sb.AppendLine($"/* delete from {table.Name} */");
                sb.AppendLine(deleteSql);
            }

            return sb.ToString();
        }

        public virtual string BuildDropTableSql(string tableName)
        {
            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                tableName,
                this.CurrentOpeningIdentifierDelimiter);

            var sql = $@"DROP TABLE {decoratedTableName}{this.EffectiveClauseTerminator}";
            return sql;
        }

        public virtual string BuildDropAllTablesSql(IDbConnection connection, bool addComments)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            var dbInspector = this.CreateDbInspector(connection);
            var tables = dbInspector.GetOrderedTableMolds(false);

            var sb = new StringBuilder();

            foreach (var table in tables)
            {
                if (addComments)
                {
                    sb.AppendLine($@"/* drop table: {table.Name} */");
                }

                var deleteScript = this.BuildDropTableSql(table.Name);
                sb.AppendLine(deleteScript);
            }

            return sb.ToString();
        }

        #endregion

        #region Static

        public static string[] SplitSqlByComments(string sql)
        {
            var statements = Regex.Split(sql, @"/\*.*?\*/", RegexOptions.Singleline)
                .Select(x => x.Trim())
                .Where(x => x != string.Empty)
                .ToArray();

            return statements;
        }

        #endregion
    }
}
