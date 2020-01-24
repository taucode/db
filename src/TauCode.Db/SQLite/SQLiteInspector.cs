using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TauCode.Db.SQLite
{
    public class SQLiteInspector : DbInspectorBase
    {
        #region Constructor

        public SQLiteInspector(IDbConnection connection)
            : base(connection)
        {
        }

        #endregion

        #region Overridden

        public override IUtilityFactory Factory => SQLiteUtilityFactory.Instance;

        protected override IReadOnlyList<string> GetTableNamesImpl()
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

                return DbUtils
                    .GetCommandRows(command)
                    .Select(x => (string)x.TableName)
                    .ToArray();
            }
        }

        #endregion
    }
}
