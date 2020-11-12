using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TauCode.Algorithms.Graphs;
using TauCode.Db.Data;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public static class DbTools
    {
        private static readonly HashSet<Type> IntegerTypes;

        private static readonly HashSet<Type> NumericTypes;

        static DbTools()
        {
            IntegerTypes = new HashSet<Type>(new[]
            {
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
            });

            var nonIntegerTypes = new[]
            {
                typeof(float),
                typeof(double),
                typeof(decimal),
            };

            var list = new List<Type>(IntegerTypes);
            list.AddRange(nonIntegerTypes);

            NumericTypes = list.ToHashSet();
        }

        public static bool IsIntegerType(this Type type) => IntegerTypes.Contains(type ?? throw new ArgumentNullException(nameof(type)));

        public static bool IsNumericType(this Type type) => NumericTypes.Contains(type ?? throw new ArgumentNullException(nameof(type)));

        public static IList<string> SplitScriptByComments(string script)
        {
            var statements = Regex.Split(script, @"/\*.*?\*/", RegexOptions.Singleline)
                .Select(x => x.Trim())
                .Where(x => x != string.Empty)
                .ToArray();

            return statements;
        }

        public static int ExecuteSingleSql(this IDbConnection connection, string sql)
        {
            using var command = connection.CreateCommand();

            command.CommandText = sql;
            return command.ExecuteNonQuery();
        }

        public static IList<dynamic> GetCommandRows(
            this IDbCommand command,
            IDbTableValuesConverter tableValuesConverter = null)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            using var reader = command.ExecuteReader();
            var rows = new List<dynamic>();

            while (reader.Read())
            {
                var row = new DynamicRow(true);

                for (var i = 0; i < reader.FieldCount; i++)
                {
                    var name = reader.GetName(i);
                    var value = reader[i];

                    if (tableValuesConverter == null)
                    {
                        // only simplest obvious transformation
                        if (value == DBNull.Value)
                        {
                            value = null;
                        }
                    }
                    else
                    {
                        var dbValueConverter = tableValuesConverter.GetColumnConverter(name);

                        if (dbValueConverter == null)
                        {
                            throw new TauDbException(
                                $"Method '{tableValuesConverter.GetType().FullName}.{nameof(IDbTableValuesConverter.GetColumnConverter)}' returned null for field '{name}'.");
                        }

                        var convertedValue = dbValueConverter.FromDbValue(value);

                        if (convertedValue == DBNull.Value)
                        {
                            throw new TauDbException(
                                $"Method '{dbValueConverter.GetType().FullName}.{nameof(IDbValueConverter.FromDbValue)}' returned 'DBNull.Value' for field '{name}'.");
                        }

                        if (convertedValue == null && value != DBNull.Value)
                        {
                            throw new TauDbException(
                                $"Method '{dbValueConverter.GetType().FullName}.{nameof(IDbValueConverter.FromDbValue)}' returned null for field '{name}' while original DB value was not <NULL>.");
                        }

                        value = convertedValue;
                    }

                    row.SetValue(name, value);
                }

                rows.Add(row);
            }

            return rows;
        }

        public static ColumnMold GetPrimaryKeySingleColumn(this TableMold table, string tableParameterName = null)
        {
            if (table == null)
            {
                throw new ArgumentNullException(nameof(table));
            }

            tableParameterName ??= nameof(table);

            if (table.PrimaryKey == null)
            {
                throw new ArgumentException($"Table '{table.Name}' does not have a primary key.", tableParameterName);
            }

            try
            {
                return table.Columns.Single(x => x.Name == table.PrimaryKey.Columns.Single());
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    $"Failed to retrieve single primary key column name for the table '{table.Name}'.",
                    tableParameterName,
                    ex);
            }
        }

        public static void ExecuteCommentedScript(this IDbConnection connection, string script)
        {
            var sqls = SplitScriptByComments(script);

            foreach (var sql in sqls)
            {
                using var command = connection.CreateCommand();
                command.CommandText = sql;
                command.ExecuteNonQuery();
            }
        }

        public static void DeleteDataFromAllTables(this IDbInspector dbInspector)
        {
            // todo: check arg, here & anywhere in this class.

            var schemaExplorer = dbInspector.Factory.CreateSchemaExplorer(dbInspector.Connection);
            var tableNames = schemaExplorer.GetTableNames(dbInspector.SchemaName, false);

            var scriptBuilder = dbInspector.Factory.CreateScriptBuilder(dbInspector.SchemaName);

            foreach (var tableName in tableNames)
            {
                var sql = scriptBuilder.BuildDeleteScript(tableName);
                dbInspector.Connection.ExecuteSingleSql(sql);
            }
        }

        public static void AddParameterWithValue(
            this IDbCommand command,
            string parameterName,
            object parameterValue)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            command.Parameters.Add(parameter);
        }

        public static TauDbException CreateUnknownDbTypeException(string type)
        {
            throw new TauDbException($"Unknown DB type exception: '{type}'.");
        }

        public static TauDbException CreateSchemaDoesNotExistException(string schemaName)
        {
            return new TauDbException($"Schema '{schemaName}' does not exist.");
        }

        public static TauDbException CreateTableDoesNotExistException(string schemaName, string tableName)
        {
            var sb = new StringBuilder();
            sb.Append($"Table '{tableName}' does not exist");

            if (schemaName == null)
            {
                sb.Append(".");
            }
            else
            {
                sb.Append($" in schema '{schemaName}'.");
            }

            return new TauDbException(sb.ToString());
        }

        public static TauDbException CreateInternalErrorException()
        {
            return new TauDbException("Internal error.");
        }

        internal static void CheckNotNullOrCorrupted(this TableMold table, string tableArgumentName)
        {
            if (table == null)
            {
                throw new ArgumentNullException(tableArgumentName);
            }

            // name
            if (table.Name == null)
            {
                throw new ArgumentException("Table name cannot be null.", tableArgumentName);
            }

            // columns
            if (table.Columns == null)
            {
                throw new ArgumentException("Table columns cannot be null.", tableArgumentName);
            }

            if (table.Columns.Any(x => x == null))
            {
                throw new ArgumentException("Table columns cannot contain nulls.", tableArgumentName);
            }

            if (table.Columns.Count == 0)
            {
                throw new ArgumentException("Table columns cannot be empty.", tableArgumentName);
            }

            foreach (var column in table.Columns)
            {
                column.CheckNotCorrupted(tableArgumentName);
            }

            // pk
            table.PrimaryKey.CheckNotCorrupted("table", true);

            // fk-s
            if (table.ForeignKeys == null)
            {
                throw new ArgumentException("Table foreign keys list cannot be null.", tableArgumentName);
            }

            if (table.ForeignKeys.Any(x => x == null))
            {
                throw new ArgumentException("Table foreign keys cannot contain nulls.", tableArgumentName);
            }

            foreach (var foreignKey in table.ForeignKeys)
            {
                foreignKey.CheckNotCorrupted(tableArgumentName);
            }

            // indexes
            if (table.Indexes == null)
            {
                throw new ArgumentException("Table indexes list cannot be null.", tableArgumentName);
            }

            if (table.Indexes.Any(x => x == null))
            {
                throw new ArgumentException("Table indexes cannot contain nulls.", tableArgumentName);
            }

            foreach (var index in table.Indexes)
            {
                index.CheckNotCorrupted(tableArgumentName);
            }
        }

        internal static void CheckNotCorrupted(this ColumnMold column, string argumentName)
        {
            if (argumentName == null)
            {
                throw new ArgumentNullException(nameof(argumentName));
            }

            if (column.Name == null)
            {
                throw new ArgumentException("Column name cannot be null.", argumentName);
            }

            if (column.Type == null)
            {
                throw new ArgumentException("Column type cannot be null.", argumentName);
            }

            column.Type.CheckNotCorrupted(argumentName);
            column.Identity?.CheckNotCorrupted(argumentName);
        }

        internal static void CheckNotCorrupted(this DbTypeMold type, string argumentName)
        {
            if (argumentName == null)
            {
                throw new ArgumentNullException(nameof(argumentName));
            }

            if (type.Name == null)
            {
                throw new ArgumentException("Type name cannot be null.", argumentName);
            }

            if (type.Size.HasValue)
            {
                if (type.Precision.HasValue || type.Scale.HasValue)
                {
                    throw new ArgumentException(
                        "If type size is provided, neither precision nor scale cannot be provided.", argumentName);
                }
            }

            if (type.Scale.HasValue && !type.Precision.HasValue)
            {
                throw new ArgumentException("If type scale is provided, precision must be provided as well.",
                    argumentName);
            }
        }

        internal static void CheckNotCorrupted(this ColumnIdentityMold columnIdentity, string argumentName)
        {
            if (argumentName == null)
            {
                throw new ArgumentNullException(nameof(argumentName));
            }

            if (columnIdentity.Seed == null)
            {
                throw new ArgumentException("Identity seed cannot be null.", argumentName);
            }

            if (columnIdentity.Increment == null)
            {
                throw new ArgumentException("Identity increment cannot be null.", argumentName);
            }
        }

        internal static void CheckNotCorrupted(this IndexMold index, string argumentName)
        {
            if (argumentName == null)
            {
                throw new ArgumentNullException(nameof(argumentName));
            }

            if (index.Name == null)
            {
                throw new ArgumentException("Index name cannot be null.", argumentName);
            }

            if (index.TableName == null)
            {
                throw new ArgumentException("Index table name cannot be null.", argumentName);
            }

            if (index.Columns == null)
            {
                throw new ArgumentException("Index columns cannot be null.", argumentName);
            }

            if (index.Columns.Count == 0)
            {
                throw new ArgumentException("Index columns cannot be empty.", argumentName);
            }

            if (index.Columns.Any(x => x == null))
            {
                throw new ArgumentException("Index columns cannot contain nulls.", argumentName);
            }

            if (index.Columns.Any(x => x.Name == null))
            {
                throw new ArgumentException("Index column name cannot be null.", argumentName);
            }
        }

        internal static void CheckNotCorrupted(this ForeignKeyMold foreignKey, string argumentName)
        {
            if (argumentName == null)
            {
                throw new ArgumentNullException(nameof(argumentName));
            }

            if (foreignKey.Name == null)
            {
                throw new ArgumentException("Foreign key name cannot be null.", argumentName);
            }

            if (foreignKey.ColumnNames == null)
            {
                throw new ArgumentException("Foreign key column names collection cannot be null.", argumentName);
            }

            if (foreignKey.ColumnNames.Count == 0)
            {
                throw new ArgumentException("Foreign key column names collection cannot be empty.", argumentName);
            }

            if (foreignKey.ColumnNames.Any(x => x == null))
            {
                throw new ArgumentException("Foreign key column names cannot contain nulls.", argumentName);
            }

            if (foreignKey.ReferencedTableName == null)
            {
                throw new ArgumentException("Foreign key's referenced table name cannot be null.", argumentName);
            }

            if (foreignKey.ReferencedColumnNames == null)
            {
                throw new ArgumentException("Foreign key referenced column names collection cannot be null.",
                    argumentName);
            }

            if (foreignKey.ReferencedColumnNames.Any(x => x == null))
            {
                throw new ArgumentException("Foreign key referenced column names cannot contain nulls.", argumentName);
            }

            if (foreignKey.ColumnNames.Count != foreignKey.ReferencedColumnNames.Count)
            {
                throw new ArgumentException(
                    "Foreign key's column name count does not match referenced column name count.", argumentName);
            }
        }

        internal static void CheckNotCorrupted(this PrimaryKeyMold primaryKey, string argumentName, bool canBeNull)
        {
            if (argumentName == null)
            {
                throw new ArgumentNullException(nameof(argumentName));
            }

            if (primaryKey == null)
            {
                if (!canBeNull)
                {
                    throw new ArgumentNullException(argumentName);
                }

                return;
            }

            if (primaryKey.Name == null)
            {
                throw new ArgumentException("Primary key's name cannot be null.", argumentName);
            }

            if (primaryKey.Columns == null)
            {
                throw new ArgumentException("Primary key's columns cannot be null.", argumentName);
            }

            if (primaryKey.Columns.Count == 0)
            {
                throw new ArgumentException("Primary key's columns cannot be empty.", argumentName);
            }

            if (primaryKey.Columns.Any(x => x == null))
            {
                throw new ArgumentException("Primary key's columns cannot contain nulls.", argumentName);
            }
        }

        public static List<TableMold> ArrangeTables(List<TableMold> tables, bool independentFirst)
        {
            var graph = new Graph<TableMold>();

            foreach (var tableMold in tables)
            {
                graph.AddNode(tableMold);
            }

            foreach (var node in graph.Nodes)
            {
                var table = node.Value;
                foreach (var foreignKey in table.ForeignKeys)
                {
                    var referencedNode =
                        graph.Nodes.SingleOrDefault(x => x.Value.Name == foreignKey.ReferencedTableName);

                    if (referencedNode == null)
                    {
                        throw CreateTableDoesNotExistException(null, foreignKey.ReferencedTableName);
                    }

                    node.DrawEdgeTo(referencedNode);
                }
            }

            var algorithm = new GraphSlicingAlgorithm<TableMold>(graph);
            var slices = algorithm.Slice();
            if (!independentFirst)
            {
                slices = slices.Reverse().ToArray();
            }

            var list = new List<TableMold>();

            foreach (var slice in slices)
            {
                var sliceTables = slice.Nodes
                    .Select(x => x.Value)
                    .OrderBy(x => x.Name);

                list.AddRange(sliceTables);
            }

            return list;
        }
    }
}
