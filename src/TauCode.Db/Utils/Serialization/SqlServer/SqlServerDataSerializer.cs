using System;
using System.Data;
using System.Linq;
using TauCode.Db.Model;
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

        protected override ParameterInfo GetParameterInfo(TableMold tableMold, string columnName)
        {
            var parameterInfo = base.GetParameterInfo(tableMold, columnName);

            if (parameterInfo == null)
            {
                var column = tableMold.Columns.Single(x =>
                    string.Equals(x.Name, columnName, StringComparison.InvariantCultureIgnoreCase));

                var typeName = column.Type.Name.ToLower();

                switch (typeName)
                {
                    case "money":
                        parameterInfo = new ParameterInfo
                        {
                            DbType = DbType.Decimal,
                            Precision = 19,
                            Scale = 4, // todo: constants?
                        };
                        break;

                    default:
                        parameterInfo = null;
                        break;
                }
            }

            return parameterInfo;
        }
    }
}
