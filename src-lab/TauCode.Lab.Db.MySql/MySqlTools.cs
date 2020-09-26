using System;
using System.Data;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.MySql
{
    public static class MySqlTools
    {
        internal const string TableTypeForTable = "BASE TABLE"; // todo: don't need this, really

        public static PrimaryKeyMold LoadPrimaryKey(IDbConnection connection, /*string dbName,*/ string tableName)
        {
            var dbName = connection.GetDatabaseName();

            // todo check args
            // todo: schema not used

            using var command = connection.CreateCommand();

            command.CommandText =
                @"
SELECT
    TC.constraint_name    ConstraintName,
    KCU.column_name       ColumnName,
    KCU.ordinal_position  OrdinalPosition
FROM
    information_schema.table_constraints TC
INNER JOIN
    information_schema.key_column_usage KCU
ON
    KCU.table_name = TC.table_name AND
    KCU.constraint_name = TC.constraint_name
WHERE
    TC.constraint_schema = @p_dbName AND
    TC.table_schema = @p_dbName AND

    KCU.constraint_schema = @p_dbName AND
    KCU.table_schema = @p_dbName AND

    TC.table_name = @p_tableName AND
    TC.constraint_type = 'PRIMARY KEY'
ORDER BY
    KCU.ordinal_position
";
            command.AddParameterWithValue("p_dbName", dbName);
            command.AddParameterWithValue("p_tableName", tableName);

            var rows = DbTools.GetCommandRows(command);
            if (rows.Count == 0)
            {
                return null;
            }

            var pkName = rows[0].ConstraintName;
            var pk = new PrimaryKeyMold
            {
                Name = pkName,
                Columns = rows
                    .Select(x => new IndexColumnMold
                    {
                        Name = (string)x.ColumnName,
                        SortDirection = SortDirection.Ascending,
                    })
                    .ToList(),
            };

            if (pk.Columns.Count != 1)
            {
                throw new NotImplementedException();
            }

            return pk;
        }

        internal static string GetDatabaseName(this IDbConnection connection)
        {
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT DATABASE() FROM DUAL;";

            var name = (string)command.ExecuteScalar();
            return name;
        }
    }
}
