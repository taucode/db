using MySql.Data.MySqlClient;
using System;
using System.Data;
using TauCode.Db;
using TauCode.Db.DbValueConverters;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlCruder : DbCruderBase
    {
        public MySqlCruder(IDbConnection connection)
            : base(connection, null)
        {
        }

        public override IDbUtilityFactory Factory => MySqlUtilityFactory.Instance;

        protected override IDbValueConverter CreateDbValueConverter(ColumnMold column)
        {
            var typeName = column.Type.Name.ToLowerInvariant();
            switch (typeName)
            {
                case "char":
                case "varchar":
                case "nchar":
                case "nvarchar":
                case "text":
                    return new StringValueConverter();

                case "int":
                case "integer":
                    return new Int32ValueConverter();

                case "timestamp":
                case "date":
                case "datetime":
                    return new DateTimeValueConverter();

                case "bit":
                    return new BooleanValueConverter();

                case "binary":
                case "varbinary":
                    return new ByteArrayValueConverter();

                case "double":
                    return new DoubleValueConverter();

                case "real":
                case "float":
                    return new SingleValueConverter();

                case "money":
                case "decimal":
                case "numeric":
                    return new DecimalValueConverter();

                case "tinyint":
                    return new SByteValueConverter();

                case "smallint":
                    return new Int16ValueConverter();

                case "bigint":
                    return new Int64ValueConverter();

                default:
                    throw new NotImplementedException();
            }
        }

        protected override IDbDataParameter CreateParameter(string tableName, ColumnMold column, string parameterName)
        {
            switch (column.Type.Name)
            {
                case "tinyint":
                    return new MySqlParameter(parameterName, MySqlDbType.Byte);

                case "smallint":
                    return new MySqlParameter(parameterName, MySqlDbType.Int16);

                case "int":
                    return new MySqlParameter(parameterName, MySqlDbType.Int32);

                case "bigint":
                    return new MySqlParameter(parameterName, MySqlDbType.Int64);

                case "float":
                    return new MySqlParameter(parameterName, MySqlDbType.Float);

                case "double":
                    return new MySqlParameter(parameterName, MySqlDbType.Double);

                case "decimal":
                    var parameter = new MySqlParameter(parameterName, MySqlDbType.Decimal);
                    parameter.Scale = (byte)(column.Type.Scale ?? 0);
                    parameter.Precision = (byte)(column.Type.Precision ?? 0);
                    return parameter;

                case "char":
                    return new MySqlParameter(
                        parameterName,
                        MySqlDbType.String,
                        column.Type.Size ?? throw new NotImplementedException());

                case "varchar":
                    return new MySqlParameter(
                        parameterName,
                        MySqlDbType.VarChar,
                        column.Type.Size ?? throw new NotImplementedException());

                case "text":
                    return new MySqlParameter(parameterName, MySqlDbType.Text);

                case "varbinary":
                    return new MySqlParameter(
                        parameterName,
                        MySqlDbType.Blob,
                        column.Type.Size ?? throw new NotImplementedException());

                case "datetime":
                    return new MySqlParameter(parameterName, MySqlDbType.DateTime);

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
