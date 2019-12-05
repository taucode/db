using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using TauCode.Db.Data;
using TauCode.Db.Model;

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

        public static IDbConnection CreateConnection(string dbProviderName)
        {
            throw new NotImplementedException();
        }

        public static IUtilityFactory GetUtilityFactory(string dbProviderName)
        {
            throw new NotImplementedException();
        }
    }
}
