using System;
using System.Collections.Generic;
using System.Data;
using TauCode.Db;
using TauCode.Db.DbValueConverters;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.Npgsql
{
    public class NpgsqlCruder : DbCruderBase
    {
        #region Constants

        protected const int MoneyTypePrecision = 19;
        protected const int MoneyTypeScale = 4;

        #endregion

        public NpgsqlCruder(IDbConnection connection, string schema)
            : base(connection, schema)
        {
        }

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactory.Instance;

        protected override IDbValueConverter CreateDbValueConverter(ColumnMold column)
        {
            var typeName = column.Type.Name.ToLowerInvariant();
            switch (typeName)
            {
                case "uuid":
                    return new GuidValueConverter();

                case "character":
                case "character varying":
                case "text":
                    return new StringValueConverter();

                case "integer":
                    return new Int32ValueConverter();

                case "date":
                    return new DateTimeValueConverter();

                case "bit":
                    return new BooleanValueConverter();

                case "float":
                    return new DoubleValueConverter();

                case "real":
                    return new SingleValueConverter();

                case "money":
                case "decimal":
                case "numeric":
                    return new DecimalValueConverter();

                case "double precision":
                    return new DoubleValueConverter();

                case "smallint":
                    return new Int16ValueConverter();

                case "bigint":
                    return new Int64ValueConverter();

                case "timestamp without time zone":
                    return new DateTimeValueConverter();

                case "boolean":
                    return new BooleanValueConverter();

                case "bytea":
                    return new ByteArrayValueConverter();

                default:
                    throw new NotImplementedException();
            }
        }

        protected override IDbParameterInfo ColumnToParameterInfo(
            string columnName,
            DbTypeMold columnType,
            IReadOnlyDictionary<string, string> parameterNameMappings)
        {
            var typeName = columnType.Name.ToLowerInvariant();

            DbType dbType;
            int? size = null;
            int? precision = null;
            int? scale = null;
            var parameterName = parameterNameMappings[columnName];

            switch (typeName)
            {
                case "integer":
                    dbType = DbType.Int32;
                    break;

                case "smallint":
                    dbType = DbType.Int16;
                    break;

                case "bigint":
                    dbType = DbType.Int64;
                    break;

                case "text":
                    dbType = DbType.String;
                    size = -1;
                    break;

                case "double precision":
                    dbType = DbType.Double;
                    break;

                case "real":
                    dbType = DbType.Single;
                    break;

                case "money":
                case "numeric":
                    dbType = DbType.Decimal;
                    break;

                case "character":
                case "character varying":
                    dbType = DbType.String;
                    size = columnType.Size;
                    break;

                case "uuid":
                    dbType = DbType.Guid;
                    break;

                case "timestamp without time zone":
                    dbType = DbType.DateTime;
                    break;

                case "boolean":
                    dbType = DbType.Boolean;
                    break;

                case "bytea":
                    dbType = DbType.Binary;
                    size = -1;
                    break;

                default:
                    throw new NotImplementedException();
            }

            var result = new DbParameterInfo(parameterName, dbType, size, precision, scale);
            return result;

            //var result = base.ColumnToParameterInfo(columnName, columnType, parameterNameMappings);

            //if (result == null)
            //{
            //    DbType dbType;



            //    NpgsqlDbType dea;

            //    int? size = null;
            //    int? precision = null;
            //    int? scale = null;
            //    var parameterName = parameterNameMappings[columnName];

            //    var typeName = columnType.Name.ToLowerInvariant();

            //    switch (typeName)
            //    {
            //        case "character varying":
            //            dbType = DbType.String;
            //            size = columnType.Size;
            //            break;

            //        case "money":
            //            dbType = DbType.Currency;
            //            precision = MoneyTypePrecision;
            //            scale = MoneyTypeScale;
            //            break;

            //        default:
            //            return null;
            //    }

            //    result = new DbParameterInfo(parameterName, dbType, size, precision, scale);
            //}

            //return result;
        }
    }
}
