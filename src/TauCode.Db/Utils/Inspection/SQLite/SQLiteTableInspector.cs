using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Db.Utils.Dialects;
using TauCode.Db.Utils.Dialects.SQLite;

namespace TauCode.Db.Utils.Inspection.SQLite
{
    public sealed class SQLiteTableInspector : ITableInspector
    {
        #region Fields

        private readonly IDbConnection _connection;

        #endregion

        #region Constructor

        public SQLiteTableInspector(
            IDbConnection connection,
            string tableName)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            this.TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        }

        #endregion

        #region Private

        private string GetTableCreationSqlFromDb()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
@"
SELECT
    T.name  Name,
    T.sql   Sql
FROM
    sqlite_master T
WHERE
    T.type = @p_type
    AND
    T.name = @p_tableName
";

                command.AddParameterWithValue("p_type", "table");
                command.AddParameterWithValue("p_tableName", this.TableName);

                return UtilsHelper
                    .GetCommandRows(command)
                    .Single()
                    .Sql;
            }
        }

        #endregion

        #region Overridden

        public IDialect Dialect => SQLiteDialect.Instance;

        public string TableName { get; }

        public List<ColumnMold> GetColumnMolds()
        {
            return this.GetTableMold().Columns;
        }

        public PrimaryKeyMold GetPrimaryKeyMold()
        {
            return this.GetTableMold().PrimaryKey;
        }

        public List<ForeignKeyMold> GetForeignKeyMolds()
        {
            return this.GetTableMold().ForeignKeys;
        }

        public List<IndexMold> GetIndexMolds()
        {
            using (var command = _connection.CreateCommand())
            {
                command.CommandText =
@"
SELECT
    T.[name]    Name,
    T.[sql]     Sql
FROM
    sqlite_master T
WHERE
    T.[type] = @p_type
    AND
    T.[tbl_name] = @p_tableName
    AND
    T.[name] NOT LIKE @p_antiPattern
";
                command.AddParameterWithValue("p_type", "index");
                command.AddParameterWithValue("p_tableName", this.TableName);
                command.AddParameterWithValue("p_antiPattern", "sqlite_autoindex_%");

                var parser = SQLiteParser.Instance;

                var indexes = UtilsHelper
                    .GetCommandRows(command)
                    .Select(x => (IndexMold)parser.Parse((string)x.Sql).Single())
                    .ToList();

                return indexes;
            }
        }

        public TableMold GetTableMold()
        {
            var sql = this.GetTableCreationSqlFromDb();
            var parser = SQLiteParser.Instance;
            var table = (TableMold)parser.Parse(sql).Single();

            table.Indexes = this.GetIndexMolds();

            return table;
        }

        #endregion
    }
}
