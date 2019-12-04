using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TauCode.Db.SqlServer
{
    // todo: sort to regions
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

        #region Overridden

        //protected override string TableTypeForTable => "BASE TABLE";

        //protected override SqlServerTableInspectorBase CreateTableInspectorImpl(string realTableName)
        //{
        //    return new SqlServerTableInspector(this.Connection, realTableName);
        //}

        //protected override ICruder CreateCruder()
        //{
        //    return new SqlServerCruder(this.Connection);
        //}

        //public override IScriptBuilder CreateScriptBuilder()
        //{
        //    return new SqlServerScriptBuilder
        //    {
        //        CurrentOpeningIdentifierDelimiter = '[',
        //    };
        //}

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
                T.table_type = @p_tableType";

                command.AddParameterWithValue("p_tableType", TableTypeForTable);

                command.CommandText = sql;

                var tableNames = DbUtils
                    .GetCommandRows(command)
                    .Select(x => (string)x.TableName)
                    .ToArray();

                return tableNames;
            }
        }

        public override ITableInspector GetTableInspector(string tableName)
        {
            throw new System.NotImplementedException();
        }
    }
}
