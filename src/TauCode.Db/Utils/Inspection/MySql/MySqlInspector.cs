using System;
using System.Data;
using System.Linq;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Building.MySql;
using TauCode.Db.Utils.Crud;
using TauCode.Db.Utils.Crud.MySql;

namespace TauCode.Db.Utils.Inspection.MySql
{
    public class MySqlInspector : IDbInspector
    {
        #region Field

        private readonly ICruder _cruder;

        #endregion

        #region Constructor

        public MySqlInspector(IDbConnection connection)
        {
            this.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _cruder = new MySqlCruder();
        }

        #endregion

        #region IDbInspector Members

        public IDbConnection Connection { get; }

        public IScriptBuilder CreateScriptBuilder()
        {
            return new MySqlScriptBuilder();
        }

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

                return _cruder
                    .GetRows(command)
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
