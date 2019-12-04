using System;
using System.Data;
using System.Linq;

namespace TauCode.Db.SQLite
{
    public class SQLiteInspector : IDbInspector
    {
        #region Fields

        private readonly ICruder _cruder;

        #endregion

        #region Constructor

        public SQLiteInspector(IDbConnection connection)
        {
            this.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _cruder = new SQLiteCruder(this.Connection);
        }

        #endregion

        #region IDbInspector Members

        public IDbConnection Connection { get; }

        public IScriptBuilder CreateScriptBuilder()
        {
            return new SQLiteScriptBuilder();
        }

        public string[] GetTableNames()
        {
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText =
@"SELECT
    T.name TableName
FROM
    sqlite_master T
WHERE
    type = @p_type
    AND
    T.name <> @p_sequenceName";

                command.AddParameterWithValue("p_type", "table");
                command.AddParameterWithValue("p_sequenceName", "sqlite_sequence");

                return UtilsHelper
                    .GetCommandRows(command)
                    .Select(x => (string)x.TableName)
                    .ToArray();
            }
        }

        public ITableInspector GetTableInspector(string tableName)
        {
            return new SQLiteTableInspector(this.Connection, tableName);
        }

        #endregion
    }
}
