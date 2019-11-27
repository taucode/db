﻿using System;
using System.Collections.Generic;

namespace TauCode.Db.Utils.Crud
{
    // todo clean up
    public static class CruderExtensions
    {
        public static IList<dynamic> GetRows(this ICruder cruder, string tableName)
        {
            // todo check args
            var connection = cruder.DbInspector.Connection;
            var dbInspector = cruder.DbInspector;
            var tableInspector = dbInspector.GetTableInspector(tableName);
            var tableMold = tableInspector.GetTableMold();

            using (var command = connection.CreateCommand())
            {
                //var dbInspector = cruder.GetTableInspector(connection, tableName);
                //var tableMold = dbInspector.GetTableMold();
                var sql = cruder.ScriptBuilder.BuildSelectSql(tableMold);
                command.CommandText = sql;

                //return cruder.GetRows(command);
                return UtilsHelper.GetCommandRows(command);
            }
        }
    }
}
