using System;
using System.Data;
using System.Data.SQLite;
using TauCode.Db;
using TauCode.Db.DbValueConverters;
using TauCode.Db.Model;

// todo clean up
namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteCruder : DbCruderBase
    {
        private static readonly int GuidRepresentationLength = Guid.Empty.ToString().Length;

        public SQLiteCruder(IDbConnection connection)
            : base(connection, null)
        {
        }

        public override IDbUtilityFactory Factory => SQLiteUtilityFactory.Instance;

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

        protected override IDbDataParameter CreateParameter(string tableName, ColumnMold column, string parameterName)
        {
            var typeName = column.Type.Name.ToLowerInvariant();

            switch (typeName)
            {
                case "uniqueidentifier":
                    return new SQLiteParameter(parameterName, DbType.AnsiStringFixedLength, GuidRepresentationLength);

                case "text":
                    return new SQLiteParameter(parameterName, DbType.String, -1);

                case "datetime":
                    return new SQLiteParameter(parameterName, DbType.DateTime);

                case "integer":
                    return new SQLiteParameter(parameterName, DbType.Int64);

                case "numeric":
                    return new SQLiteParameter(parameterName, DbType.Decimal);

                case "real":
                    return new SQLiteParameter(parameterName, DbType.Double);

                case "blob":
                    return new SQLiteParameter(parameterName, DbType.Binary, -1);

                default:
                    throw new NotImplementedException();
            }
        }


        //protected override IDbParameterInfo ColumnToParameterInfo(
        //    string columnName,
        //    DbTypeMold columnType,
        //    IReadOnlyDictionary<string, string> parameterNameMappings)
        //{
        //    DbType dbType;
        //    int? size = null;
        //    int? precision = null;
        //    int? scale = null;
        //    var parameterName = parameterNameMappings[columnName];

        //    var typeName = columnType.Name.ToLowerInvariant();

        //    switch (typeName)
        //    {
        //        case "uniqueidentifier":
        //            dbType = DbType.AnsiStringFixedLength;
        //            size = GuidRepresentationLength;
        //            break;

        //        case "text":
        //            dbType = DbType.String;
        //            size = -1;
        //            break;

        //        case "integer":
        //            dbType = DbType.Int64;
        //            break;

        //        case "blob":
        //            dbType = DbType.Binary;
        //            size = -1;
        //            break;

        //        default:
        //            return base.ColumnToParameterInfo(columnName, columnType, parameterNameMappings);
        //    }

        //    IDbParameterInfo parameterInfo = new DbParameterInfo(parameterName, dbType, size, precision, scale);
        //    return parameterInfo;
        //}
    }
}
