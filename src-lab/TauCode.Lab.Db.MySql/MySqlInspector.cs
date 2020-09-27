using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlInspector : DbInspectorBase
    {
        #region Constants

        

        #endregion

        #region Constructor

        public MySqlInspector(IDbConnection connection)
            : base(connection, null)
        {
        }

        #endregion

        public override IDbUtilityFactory Factory => MySqlUtilityFactory.Instance;

        protected override IReadOnlyList<string> GetTableNamesImpl(string schema)
        {
            var dbName = this.Connection.GetDatabaseName();

            using var command = this.Connection.CreateCommand();
            var sql =
                @"
SELECT
    T.table_name TableName
FROM
    information_schema.tables T
WHERE
    T.table_type = @p_tableType
    AND
    T.table_schema = @p_dbName;
";

            command.AddParameterWithValue("p_tableType", MySqlTools.TableTypeForTable);
            command.AddParameterWithValue("p_dbName", dbName);

            command.CommandText = sql;

            var tableNames = DbTools
                .GetCommandRows(command)
                .Select(x => (string)x.TableName)
                .ToArray();

            return tableNames;
        }
    }
}
