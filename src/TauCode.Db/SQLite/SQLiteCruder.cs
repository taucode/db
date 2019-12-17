using System;
using System.Collections.Generic;
using System.Data;
using TauCode.Db.Model;

namespace TauCode.Db.SQLite
{
    public class SQLiteCruder : CruderBase
    {
        public SQLiteCruder(IDbConnection connection)
            : base(connection)
        {
        }

        public override IUtilityFactory Factory => SQLiteUtilityFactory.Instance;

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
                    size = Guid.Empty.ToString().Length; // todo constant
                    break;

                case "text":
                    dbType = DbType.String;
                    size = -1;
                    break;

                default:
                    return base.ColumnToParameterInfo(columnName, columnType, parameterNameMappings);
            }

            IParameterInfo parameterInfo = new ParameterInfoImpl(parameterName, dbType, size, precision, scale);
            return parameterInfo;
        }

        protected override object TransformOriginalColumnValue(IParameterInfo parameterInfo, object originalColumnValue)
        {
            if (originalColumnValue == null)
            {
                return base.TransformOriginalColumnValue(parameterInfo, originalColumnValue);
            }

            switch (parameterInfo.DbType)
            {
                case DbType.AnsiStringFixedLength:
                    if (originalColumnValue is Guid guid)
                    {
                        return guid.ToString();
                    }
                    else
                    {
                        return base.TransformOriginalColumnValue(parameterInfo, originalColumnValue);
                    }

                default:
                    return base.TransformOriginalColumnValue(parameterInfo, originalColumnValue);
            }
        }
    }
}
