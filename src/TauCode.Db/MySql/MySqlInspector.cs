using System;
using System.Data;
using System.Linq;

namespace TauCode.Db.MySql
{
    public class MySqlInspector : IDbInspector
    {
        #region Constructor

        public MySqlInspector(IDbConnection connection)
        {
            this.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        #endregion

        #region IDbInspector Members

        public IDbConnection Connection { get; }

        public IScriptBuilder CreateScriptBuilder() => new MySqlScriptBuilder();

        public string[] GetTableNames()
        {
            using (var command = this.Connection.CreateCommand())
            {
                var sql =
$@"
SELECT
    T.table_name TableName
FROM
    information_schema.tables T
WHERE
    T.table_schema = @p_schemaName";

                command.CommandText = sql;
                command.AddParameterWithValue("p_schemaName", this.Connection.Database);

                return UtilsHelper
                    .GetCommandRows(command)
                    .Select(x => (string)x.TableName)
                    .ToArray();
            }
        }

        public ITableInspector GetTableInspector(string tableName)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var realTableNames = this.GetTableNames();
            var realTableName = realTableNames.Single(x => string.Equals(x, tableName, StringComparison.InvariantCultureIgnoreCase));

            return new MySqlTableInspector(this.Connection, realTableName);
        }

        #endregion
    }
}
