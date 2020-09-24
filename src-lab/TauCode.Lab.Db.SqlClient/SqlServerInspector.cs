using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlServerInspector : DbInspectorBase
    {
        #region Constants

        private const string TableTypeForTable = "BASE TABLE";

        #endregion

        #region Constructor

        public SqlServerInspector(IDbConnection connection)
            : base(connection)
        {
        }

        #endregion

        public override IUtilityFactory Factory => SqlServerUtilityFactory.Instance;

        protected override IReadOnlyList<string> GetTableNamesImpl()
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
    T.table_type = @p_tableType
";

                command.AddParameterWithValue("p_tableType", TableTypeForTable);

                command.CommandText = sql;

                var tableNames = DbTools
                    .GetCommandRows(command)
                    .Select(x => (string)x.TableName)
                    .ToArray();

                return tableNames;
            }
        }
    }
}
