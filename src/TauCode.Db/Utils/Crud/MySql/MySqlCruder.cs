using System.Data;
using TauCode.Db.Utils.Building.MySql;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.MySql;

namespace TauCode.Db.Utils.Crud.MySql
{
    public class MySqlCruder : CruderBase
    {
        protected internal MySqlCruder()
            : base(new MySqlScriptBuilder())
        {
        }

        protected override ITableInspector GetTableInspectorImpl(IDbConnection connection, string tableName)
        {
            var dbInspector = new MySqlInspector(connection);
            var tableInspector = dbInspector.GetTableInspector(tableName);
            return tableInspector;
        }
    }
}
