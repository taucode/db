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

namespace TauCode.Db
{
    public static class DbUtils
    {
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

        public static IList<dynamic> GetCommandRows(IDbCommand command)
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
                        row.SetValue(name, value);
                    }

                    rows.Add(row);
                }

                return rows;
            }
        }

        public static ColumnMold GetPrimaryKeyColumn(this TableMold table)
        {
            return table.Columns.Single(x => x.Name == table.PrimaryKey.Columns.Single().Name); // todo can throw a lot.
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
            var dialect = dbInspector.Factory.GetDialect();
            var scriptBuilder = dbInspector.Factory.CreateScriptBuilder();
            scriptBuilder.CurrentOpeningIdentifierDelimiter = dialect.IdentifierDelimiters.FirstOrDefault()?.Item1; // choose some delimiter

            foreach (var tableName in tableNames)
            {
                var sql = scriptBuilder.BuildDropTableScript(tableName);
                dbInspector.Connection.ExecuteSingleSql(sql);
            }
        }

        public static void DeleteDataFromAllTables(this IDbInspector dbInspector)
        {
            var tableNames = dbInspector.GetTableNames(false);
            var dialect = dbInspector.Factory.GetDialect();
            var scriptBuilder = dbInspector.Factory.CreateScriptBuilder();
            scriptBuilder.CurrentOpeningIdentifierDelimiter = dialect.IdentifierDelimiters.FirstOrDefault()?.Item1; // choose some delimiter

            foreach (var tableName in tableNames)
            {
                var sql = scriptBuilder.BuildDeleteScript(tableName);
                dbInspector.Connection.ExecuteSingleSql(sql);
            }
        }

        private static IDbConnection TryCreateDbConnection(string dbConnectionTypeFullName)
        {
            var allTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.Modules)
                .SelectMany(x => x.GetTypes())
                .OrderBy(x => x.FullName)
                .ToList();

            var types = allTypes
                .Where(x => x.FullName == dbConnectionTypeFullName)
                .ToList();

            if (!types.Any())
            {
                throw new TauCodeDbException($"Type '{dbConnectionTypeFullName}' was not found in the current AppDomain.");
            }

            if (types.Count() > 1)
            {
                throw new TauCodeDbException($"Current AppDomain hosts more than one type with full name '{dbConnectionTypeFullName}'.");
            }

            var type = types.Single();

            var ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor == null)
            {
                throw new TauCodeDbException($"Type '{dbConnectionTypeFullName}' doesn't have .ctor(string).");
            }

            object connectionObject;

            try
            {
                connectionObject = ctor.Invoke(new object[0]);
            }
            catch (Exception ex)
            {
                throw new TauCodeDbException($"Failed to invoke .ctor(string) of '{dbConnectionTypeFullName}'.", ex);
            }

            if (connectionObject is IDbConnection dbConnection)
            {
                return dbConnection;
            }
            else
            {
                throw new TauCodeDbException($".ctor(string) of '{dbConnectionTypeFullName}' returned not an instance of '{typeof(IDbConnection).FullName}'.");
            }
        }

        public static IDbConnection CreateConnection(string dbProviderName)
        {
            if (dbProviderName == null)
            {
                throw new ArgumentNullException(nameof(dbProviderName));
            }

            string fullTypeName = null;

            switch (dbProviderName)
            {
                case DbProviderNames.SqlServer:
                    fullTypeName = "System.Data.SqlClient.SqlConnection";
                    break;

                case DbProviderNames.SQLite:
                    fullTypeName = "System.Data.SQLite.SQLiteConnection";
                    break;

                default:
                    throw new NotSupportedException($"Cannot instantiate connection for type '{dbProviderName}'.");
            }

            var connection = TryCreateDbConnection(fullTypeName);
            return connection;
        }

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
    }
}
