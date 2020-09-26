using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using TauCode.Algorithms.Graphs;
using TauCode.Db.Data;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;

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
                        var convertedValue = dbValueConverter.FromDbValue(value);

                        if (convertedValue == DBNull.Value)
                        {
                            throw new NotImplementedException(); // error in your converter logic.
                        }

                        if (convertedValue == null && value != DBNull.Value)
                        {
                            // the only case IDbValueConverter.FromDbValue returns null is value equal to DBNull.Value.

                            throw new NotImplementedException();
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
                    dbInspector.Schema,
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
                        var referencedNode = graph.Nodes.Single(x => string.Equals(x.Value.Name, foreignKey.ReferencedTableName, StringComparison.InvariantCultureIgnoreCase));

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
                        .OrderBy(x => x.Name.ToLowerInvariant());

                    list.AddRange(sliceTables);
                }

                return list;
            }

            return tableMolds;
        }

        public static void DropAllTables(this IDbInspector dbInspector)
        {
            var tableNames = dbInspector.GetOrderedTableNames(false);
            var scriptBuilder = dbInspector.Factory.CreateScriptBuilder(dbInspector.Schema);
            
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
            var scriptBuilder = dbInspector.Factory.CreateScriptBuilder(dbInspector.Schema);
            
            foreach (var tableName in tableNames)
            {
                var sql = scriptBuilder.BuildDeleteScript(tableName);
                dbInspector.Connection.ExecuteSingleSql(sql);
            }
        }

        public static string ConvertDbToJson(
            this IDbConverter dbConverter,
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

        public static DbException CreateTableNotFoundException(string tableName)
        {
            return new DbException($"Table '{tableName}' not found.");
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
    }
}
