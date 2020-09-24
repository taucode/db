using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using TauCode.Db.Data;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;
using TauCode.Db.SQLite;
using TauCode.Db.SqlServer;

// todo: clean up
namespace TauCode.Db
{
    public static class DbUtils
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
            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                return command.ExecuteNonQuery();
            }
        }

        public static IList<dynamic> GetCommandRows(IDbCommand command, ITableValuesConverter tableValuesConverter = null)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            using (var reader = command.ExecuteReader())
            {
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
        }

        public static ColumnMold GetPrimaryKeyColumn(this TableMold table)
        {
            return table.Columns.Single(x => x.Name == table.PrimaryKey.Columns.Single().Name);
        }

        public static void ExecuteCommentedScript(this IDbConnection connection, string script)
        {
            var sqls = SplitScriptByComments(script);

            foreach (var sql in sqls)
            {
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = sql;
                    command.ExecuteNonQuery();
                }
            }
        }

        public static IList<TableMold> GetTables(
            this IDbInspector dbInspector,
            bool? independentFirst = null,
            Func<string, bool> tableNamePredicate = null)
        {
            return dbInspector
                .GetTableNames(independentFirst)
                .Where(tableNamePredicate ?? (x => true))
                .Select(x => dbInspector
                    .Factory
                    .CreateTableInspector(dbInspector.Connection, x)
                    .GetTable())
                .ToList();
        }

        public static void DropAllTables(this IDbInspector dbInspector)
        {
            var tableNames = dbInspector.GetTableNames(false);
            var scriptBuilder = dbInspector.Factory.CreateScriptBuilder();
            
            foreach (var tableName in tableNames)
            {
                var sql = scriptBuilder.BuildDropTableScript(tableName);
                dbInspector.Connection.ExecuteSingleSql(sql);
            }
        }

        public static void DeleteDataFromAllTables(this IDbInspector dbInspector)
        {
            var tableNames = dbInspector.GetTableNames(false);
            var scriptBuilder = dbInspector.Factory.CreateScriptBuilder();
            
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
            var json = DbUtils.FineSerializeToJson(convertedDb);
            return json;
        }

        //private static IDbConnection TryCreateDbConnection(string dbConnectionTypeFullName)
        //{
        //    var allTypes = AppDomain.CurrentDomain
        //        .GetAssemblies()
        //        .SelectMany(x => x.Modules)
        //        .SelectMany(x => x.GetTypes())
        //        .OrderBy(x => x.FullName)
        //        .ToList();

        //    var types = allTypes
        //        .Where(x => x.FullName == dbConnectionTypeFullName)
        //        .ToList();

        //    if (!types.Any())
        //    {
        //        throw new DbException($"Type '{dbConnectionTypeFullName}' was not found in the current AppDomain.");
        //    }

        //    if (types.Count() > 1)
        //    {
        //        throw new DbException($"Current AppDomain hosts more than one type with full name '{dbConnectionTypeFullName}'.");
        //    }

        //    var type = types.Single();

        //    var ctor = type.GetConstructor(Type.EmptyTypes);
        //    if (ctor == null)
        //    {
        //        throw new DbException($"Type '{dbConnectionTypeFullName}' doesn't have .ctor(string).");
        //    }

        //    object connectionObject;

        //    try
        //    {
        //        connectionObject = ctor.Invoke(new object[0]);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new DbException($"Failed to invoke .ctor(string) of '{dbConnectionTypeFullName}'.", ex);
        //    }

        //    if (connectionObject is IDbConnection dbConnection)
        //    {
        //        return dbConnection;
        //    }
        //    else
        //    {
        //        throw new DbException($".ctor(string) of '{dbConnectionTypeFullName}' returned not an instance of '{typeof(IDbConnection).FullName}'.");
        //    }
        //}

        //public static IDbConnection CreateConnection(string dbProviderName)
        //{
        //    if (dbProviderName == null)
        //    {
        //        throw new ArgumentNullException(nameof(dbProviderName));
        //    }

        //    string fullTypeName;

        //    switch (dbProviderName)
        //    {
        //        case DbProviderNames.SqlServer:
        //            fullTypeName = "System.Data.SqlClient.SqlConnection";
        //            break;

        //        case DbProviderNames.SQLite:
        //            fullTypeName = "System.Data.SQLite.SQLiteConnection";
        //            break;

        //        default:
        //            throw new NotSupportedException($"Cannot instantiate connection for type '{dbProviderName}'.");
        //    }

        //    var connection = TryCreateDbConnection(fullTypeName);
        //    return connection;
        //}

        public static IUtilityFactory GetUtilityFactory(string dbProviderName)
        {
            if (dbProviderName == null)
            {
                throw new ArgumentNullException(nameof(dbProviderName));
            }

            switch (dbProviderName)
            {
                case DbProviderNames.SqlServer:
                    return SqlServerUtilityFactory.Instance;

                case DbProviderNames.SQLite:
                    return SQLiteUtilityFactory.Instance;

                default:
                    throw new NotSupportedException($"Cannot create utility factory for '{dbProviderName}'.");
            }
        }

        internal static void AddParameterWithValue(
            this IDbCommand command,
            string parameterName,
            object parameterValue)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            command.Parameters.Add(parameter);
        }

        internal static void MarkAsExplicitPrimaryKey(this ColumnMold columnMold)
        {
            columnMold.SetBoolProperty("is-explicit-primary-key", true);
        }

        internal static void SetBoolProperty(this IMold mold, string propertyName, bool value)
        {
            mold.Properties[propertyName] = value.ToString();
        }

        internal static DbException CreateTableNotFoundException(string tableName)
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

        /// <summary>
        /// Boosts SQLite insertions
        /// https://stackoverflow.com/questions/3852068/sqlite-insert-very-slow
        /// </summary>
        /// <param name="sqLiteConnection">SQLite connection to boost</param>
        public static void BoostSQLiteInsertions(this IDbConnection sqLiteConnection)
        {
            if (sqLiteConnection == null)
            {
                throw new ArgumentNullException(nameof(sqLiteConnection));
            }

            using (var command = sqLiteConnection.CreateCommand())
            {
                command.CommandText = "PRAGMA journal_mode = WAL";
                command.ExecuteNonQuery();

                command.CommandText = "PRAGMA synchronous = NORMAL";
                command.ExecuteNonQuery();
            }
        }

        // todo: move this to taucode.db
        /// <summary>
        /// Creates temporary .sqlite file and returns a SQLite connection string for this file.
        /// </summary>
        /// <returns>
        /// Tuple with two strings. Item1 is temporary file path, Item2 is connection string.
        /// </returns>
        //public static Tuple<string, string> CreateSQLiteConnectionString()
        //{
        //    var tempDbFilePath = FileExtensions.CreateTempFilePath("zunit", ".sqlite");
        //    SQLiteConnection.CreateFile(tempDbFilePath);

        //    var connectionString = $"Data Source={tempDbFilePath};Version=3;";

        //    return Tuple.Create(tempDbFilePath, connectionString);
        //}

    }
}
