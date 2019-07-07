using System.Data;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Building.SqlServer;
using TauCode.Db.Utils.Crud;
using TauCode.Db.Utils.Crud.SqlServer;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.SqlServer;

namespace TauCode.Db.Utils.Serialization.SqlServer
{
    public class SqlServerDataSerializer : DataSerializerBase
    {
        public SqlServerDataSerializer()
        {
        }

        protected override ICruder CreateCruder()
        {
            return new SqlServerCruder();
        }

        protected override IScriptBuilder CreateScriptBuilder()
        {
            return new SqlServerScriptBuilder
            {
                CurrentOpeningIdentifierDelimiter = '[',
            };
        }

        protected override IDbInspector GetDbInspector(IDbConnection connection)
        {
            return new SqlServerInspector(connection);
        }
    }
}
