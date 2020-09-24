using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlInspector : DbInspectorBase
    {
        #region Constants

        private const string TableTypeForTable = "BASE TABLE";

        #endregion

        #region Constructor

        public SqlInspector(IDbConnection connection)
            : base(connection)
        {
        }

        #endregion

        public override IDbUtilityFactory Factory => SqlUtilityFactory.Instance;

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
