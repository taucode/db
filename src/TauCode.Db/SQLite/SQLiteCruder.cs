using System;
using System.Collections.Generic;
using System.Data;
using TauCode.Db.DbValueConverters;
using TauCode.Db.Model;

namespace TauCode.Db.SQLite
{
    public class SQLiteCruder : CruderBase
    {
        private static readonly int GuidRepresentationLength = Guid.Empty.ToString().Length;

        public SQLiteCruder(IDbConnection connection)
            : base(connection)
        {
        }

        public override IUtilityFactory Factory => SQLiteUtilityFactory.Instance;

        protected override IDbValueConverter CreateDbValueConverter(ColumnMold column)
        {
            var typeName = column.Type.Name.ToLowerInvariant();
            switch (typeName)
            {
                case "uniqueidentifier":
                    return new GuidValueConverter();

                case "text":
                    return new StringValueConverter();

                case "datetime":
                    return new DateTimeValueConverter();

                case "integer":
                    return new SQLiteInt64ValueConverter();

                case "blob":
                    return new ByteArrayValueConverter();

                case "real":
                    return new DoubleValueConverter();

                case "numeric":
                    return new DecimalValueConverter();

                default:
                    throw new NotSupportedException($"Type name '{typeName}' not supported.");
            }
        }

        protected override IParameterInfo ColumnToParameterInfo(
            string columnName,
            DbTypeMold columnType,
            IReadOnlyDictionary<string, string> parameterNameMappings)
        {
            DbType dbType;
            int? size = null;
            int? precision = null;
            int? scale = null;
            var parameterName = parameterNameMappings[columnName];

            var typeName = columnType.Name.ToLowerInvariant();

            switch (typeName)
            {
                case "uniqueidentifier":
                    dbType = DbType.AnsiStringFixedLength;
                    size = GuidRepresentationLength;
                    break;

                case "text":
                    dbType = DbType.String;
                    size = -1;
                    break;

                case "integer":
                    dbType = DbType.Int64;
                    break;

                case "blob":
                    dbType = DbType.Binary;
                    size = -1;
                    break;

                default:
                    return base.ColumnToParameterInfo(columnName, columnType, parameterNameMappings);
            }

            IParameterInfo parameterInfo = new ParameterInfoImpl(parameterName, dbType, size, precision, scale);
            return parameterInfo;
        }
    }
}
