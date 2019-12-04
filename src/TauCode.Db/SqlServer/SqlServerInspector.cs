using System;
using System.Data;

namespace TauCode.Db.SqlServer
{
    public class SqlServerInspector : /*SqlServerInspectorBase*/ IDbInspector
    {
        #region Constructor

        public SqlServerInspector(IDbConnection connection)
            //: base(connection)
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

        public IUtilityFactory Factory => throw new NotImplementedException();
        public string[] GetTableNames(bool? independentFirst = null)
        {
            throw new System.NotImplementedException();
        }

        public ITableInspector GetTableInspector(string tableName)
        {
            throw new System.NotImplementedException();
        }
    }
}
