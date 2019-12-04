using System;
using System.Collections.Generic;

namespace TauCode.Db
{
    public static class CruderExtensions
    {
        public static IList<dynamic> GetRows(this ICruder cruder, string tableName)
        {
            if (cruder == null)
            {
                throw new ArgumentNullException(nameof(cruder));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var connection = cruder.DbInspector.Connection;
            var dbInspector = cruder.DbInspector;
            var tableInspector = dbInspector.GetTableInspector(tableName);
            var tableMold = tableInspector.GetTableMold();

            using (var command = connection.CreateCommand())
            {
                var sql = cruder.ScriptBuilder.BuildSelectSql(tableMold);
                command.CommandText = sql;
                return UtilsHelper.GetCommandRows(command);
            }
        }
    }
}
