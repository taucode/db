using System;
using System.Collections.Generic;
using System.Data;
using TauCode.Db;
using TauCode.Db.DbValueConverters;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlCruder : DbCruderBase
    {
        #region Constants

        protected const int MoneyTypePrecision = 19;
        protected const int MoneyTypeScale = 4;

        #endregion

        public SqlCruder(IDbConnection connection)
            : base(connection)
        {
        }

        public override IDbUtilityFactory Factory => SqlUtilityFactory.Instance;

        protected override IDbValueConverter CreateDbValueConverter(ColumnMold column)
        {
            var typeName = column.Type.Name.ToLowerInvariant();
            switch (typeName)
            {
                case "uniqueidentifier":
                    return new GuidValueConverter();

                case "char":
                case "varchar":
                case "nchar":
                case "nvarchar":
                    return new StringValueConverter();

                case "int":
                case "integer":
                    return new Int32ValueConverter();

                case "datetime":
                case "datetime2":
                case "date":
                    return new DateTimeValueConverter();

                case "bit":
                    return new BooleanValueConverter();

                case "binary":
                case "varbinary":
                    return new ByteArrayValueConverter();

                case "float":
                    return new DoubleValueConverter();

                case "real":
                    return new SingleValueConverter();

                case "money":
                case "decimal":
                case "numeric":
                    return new DecimalValueConverter();

                case "tinyint":
                    return new ByteValueConverter();

                case "smallint":
                    return new Int16ValueConverter();

                case "bigint":
                    return new Int64ValueConverter();

                default:
                    throw new NotImplementedException();
            }
        }

        protected override IDbParameterInfo ColumnToParameterInfo(
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

                result = new DbParameterInfo(parameterName, dbType, size, precision, scale);
            }

            return result;
        }
    }
}
