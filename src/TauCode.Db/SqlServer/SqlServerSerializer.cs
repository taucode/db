using System.Data;

namespace TauCode.Db.SqlServer
{
    // todo: nice regions
    public class SqlServerSerializer : DbSerializerBase
    {
        #region Constants

        protected const int MoneyTypePrecision = 19;
        protected const int MoneyTypeScale = 4;

        #endregion


        //private readonly IDbConnection _connection;

        public SqlServerSerializer(IDbConnection connection)
            : base(connection)

        {
            //_connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        //protected override ICruder CreateCruder() => new SqlServerCruder(_connection);

        //protected override IScriptBuilder CreateScriptBuilder()
        //{
        //    return new SqlServerScriptBuilder
        //    {
        //        CurrentOpeningIdentifierDelimiter = '[',
        //    };
        //}


        //protected override ParameterInfo GetParameterInfo(TableMold tableMold, string columnName)
        //{
        //    var parameterInfo = base.GetParameterInfo(tableMold, columnName);

        //    if (parameterInfo == null)
        //    {
        //        var column = tableMold.Columns.Single(x =>
        //            string.Equals(x.Name, columnName, StringComparison.InvariantCultureIgnoreCase));

        //        var typeName = column.Type.Name.ToLower();

        //        switch (typeName)
        //        {
        //            case "money":
        //                parameterInfo = new ParameterInfo
        //                {
        //                    DbType = DbType.Decimal,
        //                    Precision = MoneyTypePrecision,
        //                    Scale = MoneyTypeScale,
        //                };
        //                break;

        //            default:
        //                parameterInfo = null;
        //                break;
        //        }
        //    }

        //    return parameterInfo;
        //}

        public override IUtilityFactory Factory => SqlServerUtilityFactory.Instance;
    }
}
