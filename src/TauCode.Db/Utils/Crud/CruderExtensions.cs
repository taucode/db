using System.Collections.Generic;
using System.Data;

namespace TauCode.Db.Utils.Crud
{
    public static class CruderExtensions
    {
        public static List<dynamic> GetRows(this ICruder cruder, IDbConnection connection, string tableName)
        {
            using (var command = connection.CreateCommand())
            {
                var dbInspector = cruder.GetTableInspector(connection, tableName);
                var tableMold = dbInspector.GetTableMold();
                var sql = cruder.ScriptBuilder.BuildSelectSql(tableMold);
                command.CommandText = sql;

                return cruder.GetRows(command);
            }
        }
    }
}
