using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public abstract class ScriptBuilderBase : UtilityBase, IScriptBuilder
    {
        private char? _currentOpeningIdentifierDelimiter;

        protected ScriptBuilderBase()
            : base(null, false, true)
        {
        }

        protected virtual IDialect Dialect => this.Factory.GetDialect();

        protected virtual void WriteTypeDefinitionScriptFragment(StringBuilder sb, DbTypeMold type)
        {
            var decoratedTypeName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Type, type.Name,
                this.CurrentOpeningIdentifierDelimiter);

            sb.Append(decoratedTypeName);

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
        }

        protected virtual void WriteColumnDefinitionScriptFragment(StringBuilder sb, ColumnMold column)
        {
            // name
            var decoratedColumnName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Column,
                column.Name,
                this.CurrentOpeningIdentifierDelimiter);

            sb.Append(decoratedColumnName);

            sb.Append(" ");

            this.WriteTypeDefinitionScriptFragment(sb, column.Type);

            // nullability
            sb.Append(" ");

            var nullability = column.IsNullable ? "NULL" : "NOT NULL";
            sb.Append(nullability);

            if (column.Identity != null)
            {
                sb.Append(" ");
                this.WriteIdentityScriptFragment(sb, column.Identity);
            }
        }

        protected virtual void WriteIdentityScriptFragment(StringBuilder sb, ColumnIdentityMold identity)
        {
            sb.Append($"IDENTITY({identity.Seed}, {identity.Increment})");
        }

        protected virtual void WriteForeignKeyConstraintScriptFragment(StringBuilder sb, ForeignKeyMold foreignKey)
        {
            var decoratedConstraintName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Constraint,
                foreignKey.Name,
                this.CurrentOpeningIdentifierDelimiter);

            sb.Append($"CONSTRAINT {decoratedConstraintName} FOREIGN KEY(");
            this.WriteDecoratedColumnsOverCommaScriptFragment(sb, foreignKey.ColumnNames);
            sb.Append(") REFERENCES ");

            var decoratedReferencedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                foreignKey.ReferencedTableName,
                this.CurrentOpeningIdentifierDelimiter);

            sb.Append(decoratedReferencedTableName);
            sb.Append("(");
            this.WriteDecoratedColumnsOverCommaScriptFragment(sb, foreignKey.ReferencedColumnNames);
            sb.Append(")");
        }

        protected virtual void WritePrimaryKeyConstraintScriptFragment(
            StringBuilder sb,
            PrimaryKeyMold primaryKey)
        {
            var decoratedConstraintName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Constraint,
                primaryKey.Name,
                this.CurrentOpeningIdentifierDelimiter);

            sb.Append($"CONSTRAINT {decoratedConstraintName} PRIMARY KEY(");
            this.WriteDecoratedIndexColumnsOverCommaScriptFragment(sb, primaryKey.Columns);
            sb.Append(")");
        }

        protected virtual void WriteDecoratedColumnsOverCommaScriptFragment(
            StringBuilder sb,
            IReadOnlyList<string> columnNames)
        {
            for (var i = 0; i < columnNames.Count; i++)
            {
                var columnName = columnNames[i];
                var decoratedColumnName = this.Dialect.DecorateIdentifier(
                    DbIdentifierType.Column,
                    columnName,
                    this.CurrentOpeningIdentifierDelimiter);
                sb.Append(decoratedColumnName);

                if (i < columnNames.Count - 1)
                {
                    sb.Append(", ");
                }
            }
        }

        protected virtual void WriteDecoratedIndexColumnsOverCommaScriptFragment(
            StringBuilder sb,
            IReadOnlyList<IndexColumnMold> indexColumns)
        {
            for (var i = 0; i < indexColumns.Count; i++)
            {
                var indexColumn = indexColumns[i];
                var decoratedColumnName = this.Dialect.DecorateIdentifier(
                    DbIdentifierType.Column,
                    indexColumn.Name,
                    this.CurrentOpeningIdentifierDelimiter);
                sb.Append(decoratedColumnName);

                string sorting;

                switch (indexColumn.SortDirection)
                {
                    case SortDirection.Ascending:
                        sorting = "ASC";
                        break;
                    case SortDirection.Descending:
                        sorting = "DESC";
                        break;

                    default:
                        throw new NotImplementedException();
                }

                sb.Append(" ");
                sb.Append(sorting);

                if (i < indexColumns.Count - 1)
                {
                    sb.Append(", ");
                }
            }
        }

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
                }
                else
                {
                    _currentOpeningIdentifierDelimiter = null;
                }
            }
        }

        public virtual string BuildCreateTableScript(TableMold table, bool includeConstraints)
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

                sb.Append("    ");
                this.WriteColumnDefinitionScriptFragment(sb, column);

                if (i < table.Columns.Count - 1)
                {
                    sb.AppendLine(",");
                }
            }

            if (includeConstraints)
            {
                var constraints = new List<IConstraint>();
                if (table.PrimaryKey != null)
                {
                    constraints.Add(table.PrimaryKey);
                }

                constraints.AddRange(table.ForeignKeys);

                if (constraints.Count > 0)
                {
                    sb.AppendLine(",");

                    for (var i = 0; i < constraints.Count; i++)
                    {
                        var constraint = constraints[i];

                        sb.Append("    ");

                        if (constraint is PrimaryKeyMold primaryKey)
                        {
                            this.WritePrimaryKeyConstraintScriptFragment(sb, primaryKey);
                        }
                        else if (constraint is ForeignKeyMold foreignKey)
                        {
                            this.WriteForeignKeyConstraintScriptFragment(sb, foreignKey);
                        }

                        if (i < constraints.Count - 1)
                        {
                            sb.AppendLine(",");
                        }
                    }

                    foreach (var constraint in constraints)
                    {
                    }
                }
            }

            sb.Append(")");
            return sb.ToString();
        }

        public virtual string BuildDropTableScript(string tableName)
        {
            // todo checks

            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                tableName,
                this.CurrentOpeningIdentifierDelimiter);

            return $"DROP TABLE {decoratedTableName}";
        }

        public virtual string BuildInsertScript(
            TableMold table,
            IReadOnlyDictionary<string, string> columnToParameterMappings)
        {
            // todo: check args, including count of columnToParameterMappings

            var tuples = columnToParameterMappings
                .Select(x => Tuple.Create(
                    this.Dialect.DecorateIdentifier(
                        DbIdentifierType.Column,
                        x.Key,
                        this.CurrentOpeningIdentifierDelimiter),
                    x.Value))
                .ToList();

            var sb = new StringBuilder();
            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                table.Name,
                this.CurrentOpeningIdentifierDelimiter);

            sb.AppendLine($"INSERT INTO {decoratedTableName} (");
            for (var i = 0; i < tuples.Count; i++)
            {
                var tuple = tuples[i];
                var columnName = tuple.Item1;
                sb.Append("    ");
                sb.Append(columnName);
                if (i < tuples.Count - 1)
                {
                    sb.AppendLine(",");
                }
            }

            sb.AppendLine(")");
            sb.AppendLine("VALUES (");

            for (var i = 0; i < tuples.Count; i++)
            {
                var tuple = tuples[i];
                var parameterName = tuple.Item2;
                sb.Append("    @");
                sb.Append(parameterName);
                if (i < tuples.Count - 1)
                {
                    sb.AppendLine(",");
                }
            }

            sb.Append(")");

            var sql = sb.ToString();
            return sql;
        }

        // todo clean
        public virtual string BuildUpdateScript(
            TableMold table,
            IReadOnlyDictionary<string, string> columnToParameterMappings)
        {
            var sb = new StringBuilder();
            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                table.Name,
                this.CurrentOpeningIdentifierDelimiter);

            sb.AppendLine($"UPDATE {decoratedTableName} SET");

            var idColumnName = table.GetPrimaryKeyColumn().Name.ToLowerInvariant();
            var decoratedIdColumnName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Column,
                idColumnName,
                this.CurrentOpeningIdentifierDelimiter);
            var idParameterName = columnToParameterMappings[idColumnName];

            var columnNamesToUpdate = columnToParameterMappings.Keys.Except(new[] {idColumnName}).ToList();

            for (var i = 0; i < columnNamesToUpdate.Count; i++)
            {
                var columnName = columnNamesToUpdate[i];

                var decoratedColumnName = this.Dialect.DecorateIdentifier(
                    DbIdentifierType.Column,
                    columnName,
                    this.CurrentOpeningIdentifierDelimiter);

                var parameterName = columnToParameterMappings[columnName];

                sb.Append($"    {decoratedColumnName} = @{parameterName}");

                if (i < columnNamesToUpdate.Count - 1)
                {
                    sb.AppendLine(",");
                }
            }

            sb.AppendLine();

            sb.Append($"WHERE {decoratedIdColumnName} = @{idParameterName}");
            var sql = sb.ToString();
            return sql;
        }

        public virtual string BuildSelectByIdScript(TableMold table, string idParameterName)
        {
            var sb = new StringBuilder();

            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                table.Name,
                this.CurrentOpeningIdentifierDelimiter);
            
            sb.AppendLine($"SELECT");
            for (var i = 0; i < table.Columns.Count; i++)
            {
                var column = table.Columns[i];
                var decoratedColumnName = this.Dialect.DecorateIdentifier(
                    DbIdentifierType.Column,
                    column.Name,
                    this.CurrentOpeningIdentifierDelimiter);

                sb.Append($"    {decoratedColumnName}");
                if (i < table.Columns.Count - 1)
                {
                    sb.AppendLine(",");
                }
            }

            sb.AppendLine();
            sb.AppendLine($"FROM {decoratedTableName}");
            sb.AppendLine("WHERE");
            var decoratedIdColumnName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Column,
                table.GetPrimaryKeyColumn().Name,
                this.CurrentOpeningIdentifierDelimiter);
            sb.Append($"    {decoratedIdColumnName} = @{idParameterName}");

            var sql = sb.ToString();
            return sql;
        }

        public virtual string BuildSelectAllScript(TableMold table)
        {
            var sb = new StringBuilder();

            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                table.Name,
                this.CurrentOpeningIdentifierDelimiter);

            sb.AppendLine($"SELECT");
            for (var i = 0; i < table.Columns.Count; i++)
            {
                var column = table.Columns[i];
                var decoratedColumnName = this.Dialect.DecorateIdentifier(
                    DbIdentifierType.Column,
                    column.Name,
                    this.CurrentOpeningIdentifierDelimiter);

                sb.Append($"    {decoratedColumnName}");
                if (i < table.Columns.Count - 1)
                {
                    sb.AppendLine(",");
                }
            }

            sb.AppendLine();
            sb.AppendLine($"FROM {decoratedTableName}");

            var sql = sb.ToString();
            return sql;
        }

        public virtual string BuildDeleteByIdScript(TableMold table, string idParameterName)
        {
            var sb = new StringBuilder();

            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                table.Name,
                this.CurrentOpeningIdentifierDelimiter);

            var decoratedIdColumnName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Column,
                table.GetPrimaryKeyColumn().Name,
                this.CurrentOpeningIdentifierDelimiter);

            sb.Append($"DELETE FROM {decoratedTableName} WHERE {decoratedIdColumnName} = @{idParameterName}");
            var sql = sb.ToString();
            return sql;
        }

        public virtual string BuildDeleteScript(string tableName)
        {
            // todo checks

            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                tableName,
                this.CurrentOpeningIdentifierDelimiter);

            return $"DELETE FROM {decoratedTableName}";
        }
    }
}
