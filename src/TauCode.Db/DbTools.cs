using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
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

// todo: clean up, review. some methods might be not needed.
namespace TauCode.Db
{
    public static class DbTools
    {
        private static readonly HashSet<Type> IntegerTypes = new HashSet<Type>(new[]
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

        public static bool IsIntegerType(Type type) => IntegerTypes.Contains(type);

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
            IDbCommand command,
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
                            throw new TauDbException($"Method '{dbValueConverter.GetType().FullName}.{nameof(IDbValueConverter.FromDbValue)}' returned 'DBNull.Value' for field '{name}'.");
                        }

                        if (convertedValue == null && value != DBNull.Value)
                        {
                            throw new TauDbException($"Method '{dbValueConverter.GetType().FullName}.{nameof(IDbValueConverter.FromDbValue)}' returned null for field '{name}' while original DB  value was not <NULL>.");
                        }

                        value = convertedValue;
                    }

                    row.SetValue(name, value);
                }

                rows.Add(row);
            }

            return rows;
        }

        public static ColumnMold GetPrimaryKeyColumn(this TableMold table)
        {
            return table.Columns.Single(x => x.Name == table.PrimaryKey.Columns.Single().Name);
        }

        public static IReadOnlyList<string> GetOrderedTableNames(
            this IDbInspector dbInspector,
            bool independentFirst,
            Func<string, bool> tableNamePredicate = null) =>
            dbInspector.GetTables(
                    independentFirst,
                    tableNamePredicate)
                .Select(x => x.Name)
                .ToList();

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

        public static IReadOnlyList<TableMold> GetTables(
            this IDbInspector dbInspector,
            bool? independentFirst = null,
            Func<string, bool> tableNamePredicate = null)
        {
            if (dbInspector == null)
            {
                throw new ArgumentNullException(nameof(dbInspector));
            }

            tableNamePredicate ??= x => true;

            var tableNames = dbInspector.GetTableNames();

            var tableMolds = tableNames
                .Where(tableNamePredicate)
                .Select(x => dbInspector.Factory.CreateTableInspector(
                    dbInspector.Connection,
                    dbInspector.SchemaName,
                    x))
                .Select(x => x.GetTable())
                .ToList();

            if (independentFirst.HasValue)
            {
                var graph = new Graph<TableMold>();

                foreach (var tableMold in tableMolds)
                {
                    graph.AddNode(tableMold);
                }

                foreach (var node in graph.Nodes)
                {
                    var table = node.Value;
                    foreach (var foreignKey in table.ForeignKeys)
                    {
                        var referencedNode = graph.Nodes.SingleOrDefault(x => x.Value.Name == foreignKey.ReferencedTableName);

                        if (referencedNode == null)
                        {
                            throw new NotImplementedException();
                        }

                        node.DrawEdgeTo(referencedNode);
                    }
                }

                var algorithm = new GraphSlicingAlgorithm<TableMold>(graph);
                var slices = algorithm.Slice();
                if (!independentFirst.Value)
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

            return tableMolds;
        }

        public static void DropAllTables(this IDbInspector dbInspector)
        {
            var tableNames = dbInspector.GetOrderedTableNames(false);
            var scriptBuilder = dbInspector.Factory.CreateScriptBuilder(dbInspector.SchemaName);

            foreach (var tableName in tableNames)
            {
                var sql = scriptBuilder.BuildDropTableScript(tableName);
                dbInspector.Connection.ExecuteSingleSql(sql);
            }
        }

        public static void DeleteDataFromAllTables(this IDbInspector dbInspector)
        {
            // todo: check arg, here & anywhere in this class.

            var tableNames = dbInspector.GetOrderedTableNames(false);
            var scriptBuilder = dbInspector.Factory.CreateScriptBuilder(dbInspector.SchemaName);

            foreach (var tableName in tableNames)
            {
                var sql = scriptBuilder.BuildDeleteScript(tableName);
                dbInspector.Connection.ExecuteSingleSql(sql);
            }
        }

        public static string ConvertDbToJson(
            this IDbMetadataConverter dbConverter,
            DbMold originDb,
            IReadOnlyDictionary<string, string> options = null)
        {
            if (dbConverter == null)
            {
                throw new ArgumentNullException(nameof(dbConverter));
            }

            var convertedDb = dbConverter.ConvertDb(originDb, options);
            var json = DbTools.FineSerializeToJson(convertedDb);
            return json;
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

        public static string FineSerializeToJson(object obj)
        {
            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = false,
                },
            };

            var json = JsonConvert.SerializeObject(
                obj,
                new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented,
                    Converters = new List<JsonConverter>
                    {
                        new StringEnumConverter(new CamelCaseNamingStrategy())
                    }
                });

            return json;
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
    }
}
