using System;
using System.Data;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Building.SqlServer;
using TauCode.Db.Utils.Crud;
using TauCode.Db.Utils.Crud.SqlServer;

namespace TauCode.Db.Utils.Serialization.SqlServer
{
    public class SqlServerSerializer : DbSerializerBase
    {
        private const int MONEY_TYPE_PRECISION = 19;
        private const int MONEY_TYPE_SCALE = 4;

        private readonly IDbConnection _connection;

        public SqlServerSerializer(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        protected override ICruder CreateCruder() => new SqlServerCruder(_connection);

        protected override IScriptBuilder CreateScriptBuilder()
        {
            return new SqlServerScriptBuilder
            {
                CurrentOpeningIdentifierDelimiter = '[',
            };
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
                            Precision = MONEY_TYPE_PRECISION,
                            Scale = MONEY_TYPE_SCALE,
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
