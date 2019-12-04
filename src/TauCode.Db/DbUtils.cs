using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using TauCode.Db.Data;

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

        public static int ExecuteSql(IDbConnection connection, string sql)
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
    }
}
