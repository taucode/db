using System;
using System.Data;
using System.Linq;
using TauCode.Db.Exceptions;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Crud;

namespace TauCode.Db.Utils.Inspection
{
    public abstract class SqlServerInspectorBase : IDbInspector
    {
        #region Fields

        private ICruder _cruder;

        #endregion

        #region Constructor

        protected SqlServerInspectorBase(IDbConnection connection)
        {
            this.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        #endregion

        #region Abstract

        protected abstract string TableTypeForTable { get; }

        protected abstract SqlServerTableInspectorBase CreateTableInspectorImpl(string tableName);

        protected abstract ICruder CreateCruder();

        #endregion

        #region Protected

        protected ICruder Cruder => _cruder ?? (_cruder = this.CreateCruder());

        #endregion

        #region IDbInspector Members

        public IDbConnection Connection { get; }

        public abstract IScriptBuilder CreateScriptBuilder();

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
    T.table_type = @p_tableType";

                command.AddParameterWithValue("p_tableType", this.TableTypeForTable);

                command.CommandText = sql;

                var tableNames = this.Cruder
                    .GetRows(command)
                    .Select(x => (string)x.TableName)
                    .ToArray();

               return tableNames;
            }
        }

        public ITableInspector GetTableInspector(string tableName)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            using (var command = this.Connection.CreateCommand())
            {
                var sql =
@"
SELECT
    T.table_name TableName
FROM
    information_schema.tables T
WHERE
    T.table_type = @p_tableType
    AND
    T.table_name = @p_name";

                command.CommandText = sql;

                command.AddParameterWithValue("p_tableType", this.TableTypeForTable);
                command.AddParameterWithValue("p_name", tableName);

                var row = this.Cruder
                    .GetRows(command)
                    .SingleOrDefault();

                if (row == null)
                {
                    throw new ObjectNotFoundException($"Table not found: '{tableName}'", tableName);
                }

                var realTableName = (string)row.TableName;
                var tableInspector = this.CreateTableInspectorImpl(realTableName);
                return tableInspector;
            }
        }

        #endregion
    }
}
