using System;
using System.Data;

namespace TauCode.Db.SQLite
{
    // todo clean up
    public class SQLiteInspector : IDbInspector
    {
        #region Fields

        private readonly ICruder _cruder;

        #endregion

        #region Constructor

        public SQLiteInspector(IDbConnection connection)
        {
            throw new NotImplementedException();

            //this.Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            //_cruder = new SQLiteCruder(this.Connection);
        }

        #endregion

        #region IDbInspector Members OLD

        //public IDbConnection Connection { get; }

//        public IScriptBuilder CreateScriptBuilder()
//        {
//            return new SQLiteScriptBuilder();
//        }

//        public string[] GetTableNames()
//        {
//            using (var command = this.Connection.CreateCommand())
//            {
//                command.CommandText =
//@"SELECT
//    T.name TableName
//FROM
//    sqlite_master T
//WHERE
//    type = @p_type
//    AND
//    T.name <> @p_sequenceName";

//                command.AddParameterWithValue("p_type", "table");
//                command.AddParameterWithValue("p_sequenceName", "sqlite_sequence");

//                return UtilsHelper
//                    .GetCommandRows(command)
//                    .Select(x => (string)x.TableName)
//                    .ToArray();
//            }
//        }

//        public ITableInspector GetTableInspector(string tableName)
//        {
//            return new SQLiteTableInspector(this.Connection, tableName);
//        }

        #endregion

        public IUtilityFactory Factory => throw new NotImplementedException();
        public string[] GetTableNames(bool? independentFirst = null)
        {
            throw new NotImplementedException();
        }

        public ITableInspector GetTableInspector(string tableName)
        {
            throw new NotImplementedException();
        }
    }
}
