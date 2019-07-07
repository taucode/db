using System.Data;
using TauCode.Db.Utils.Building.SQLite;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.SQLite;

namespace TauCode.Db.Utils.Crud.SQLite
{
    public class SQLiteCruder : CruderBase
    {
        public SQLiteCruder()
            : base(new SQLiteScriptBuilder())
        {
        }

        protected override ITableInspector GetTableInspectorImpl(IDbConnection connection, string tableName)
        {
            var dbInspector = new SQLiteInspector(connection);
            var tableInspector = dbInspector.GetTableInspector(tableName);
            return tableInspector;
        }
    }
}
