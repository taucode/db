using System.Collections.Generic;
using System.Data;
using TauCode.Db.Model;

namespace TauCode.Db.SqlServer
{
    public class SqlServerCruder : CruderBase
    {
        #region Constants

        protected const int MoneyTypePrecision = 19;
        protected const int MoneyTypeScale = 4;

        #endregion

        public SqlServerCruder(IDbConnection connection)
            : base(connection)
        {
        }

        public override IUtilityFactory Factory => SqlServerUtilityFactory.Instance;

        protected override IParameterInfo ColumnToParameterInfo(
            string columnName,
            DbTypeMold columnType,
            IReadOnlyDictionary<string, string> parameterNameMappings)
        {
            var result = base.ColumnToParameterInfo(columnName, columnType, parameterNameMappings);

            if (result == null)
            {
                DbType dbType;
                int? size = null;
                int? precision = null;
                int? scale = null;
                var parameterName = parameterNameMappings[columnName];

                var typeName = columnType.Name.ToLowerInvariant();

                switch (typeName)
                {
                    case "money":
                        dbType = DbType.Currency;
                        precision = MoneyTypePrecision;
                        scale = MoneyTypeScale;
                        break;

                    default:
                        return null;
                }

                result = new ParameterInfoImpl(parameterName, dbType, size, precision, scale);
            }

            return result;
        }

        protected override object TransformOriginalColumnValue(IParameterInfo parameterInfo, object originalColumnValue)
        {
            var transformed = base.TransformOriginalColumnValue(parameterInfo, originalColumnValue);

            if (transformed == null)
            {
                switch (parameterInfo.DbType)
                {
                    case DbType.Currency:
                        if (originalColumnValue is double doubleValue)
                        {
                            transformed = (decimal)doubleValue;
                        }
                        else if (originalColumnValue is decimal decimalValue)
                        {
                            transformed = originalColumnValue;
                        }
                        else
                        {
                            throw this.CreateCannotTransformException(
                                parameterInfo.DbType,
                                originalColumnValue.GetType());
                        }

                        break;
                }
            }

            return transformed;
        }
    }
}
