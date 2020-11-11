using MySql.Data.MySqlClient;
using System;
using System.Data;
using TauCode.Db;
using TauCode.Db.DbValueConverters;
using TauCode.Db.Model;
using TauCode.Lab.Db.MySql.DbValueConverters;

// todo regions
namespace TauCode.Lab.Db.MySql
{
    public class MySqlCruder : DbCruderBase
    {
        public MySqlCruder(MySqlConnection connection)
            : base(connection, connection?.Database)
        {
        }

        protected override string TransformTableName(string tableName) => tableName.ToLowerInvariant();

        public override IDbUtilityFactory Factory => MySqlUtilityFactory.Instance;

        protected override IDbValueConverter CreateDbValueConverter(ColumnMold column)
        {
            switch (column.Type.Name)
            {
                case "bit":
                    return new MySqlBitValueConverter();

                case "tinyint":
                    if (column.Type.Properties.SafeGetValue("unsigned") == "true")
                    {
                        return new ByteValueConverter();
                    }
                    else
                    {
                        return new MySqlTinyIntValueConverter();
                    }

                case "smallint":
                    if (column.Type.Properties.SafeGetValue("unsigned") == "true")
                    {
                        return new UInt16ValueConverter();
                    }
                    else
                    {
                        return new Int16ValueConverter();
                    }

                case "int":
                case "mediumint":
                    if (column.Type.Properties.SafeGetValue("unsigned") == "true")
                    {
                        return new UInt32ValueConverter();
                    }
                    else
                    {
                        return new Int32ValueConverter();
                    }


                case "bigint":
                    if (column.Type.Properties.SafeGetValue("unsigned") == "true")
                    {
                        return new UInt64ValueConverter();
                    }
                    else
                    {
                        return new Int64ValueConverter();
                    }

                case "decimal":
                    return new DecimalValueConverter();

                case "float":
                    return new SingleValueConverter();

                case "double":
                    return new DoubleValueConverter();

                case "tinytext":
                case "text":
                case "mediumtext":
                case "longtext":
                case "char":
                case "varchar":
                    return new StringValueConverter();

                case "date":
                case "datetime":
                case "timestamp":
                    return new DateTimeValueConverter();

                case "time":
                    return new TimeSpanValueConverter();

                case "year":
                    return new Int16ValueConverter();

                case "binary":
                case "varbinary":
                case "tinyblob":
                case "blob":
                case "mediumblob":
                case "longblob":
                    return new ByteArrayValueConverter();

                default:
                    throw new NotImplementedException();
            }
        }

        protected override IDbDataParameter CreateParameter(string tableName, ColumnMold column)
        {
            const string parameterName = "parameter_name_placeholder";

            switch (column.Type.Name)
            {
                case "bit":
                    return new MySqlParameter(parameterName, MySqlDbType.Bit);

                case "tinyint":
                    if (column.Type.Properties.SafeGetValue("unsigned") == "true")
                    {
                        return new MySqlParameter(parameterName, MySqlDbType.UByte);
                    }
                    else
                    {
                        return new MySqlParameter(parameterName, MySqlDbType.Byte);
                    }

                case "smallint":
                    if (column.Type.Properties.SafeGetValue("unsigned") == "true")
                    {
                        return new MySqlParameter(parameterName, MySqlDbType.UInt16);
                    }
                    else
                    {
                        return new MySqlParameter(parameterName, MySqlDbType.Int16);
                    }

                case "mediumint":
                    if (column.Type.Properties.SafeGetValue("unsigned") == "true")
                    {
                        return new MySqlParameter(parameterName, MySqlDbType.UInt24);
                    }
                    else
                    {
                        return new MySqlParameter(parameterName, MySqlDbType.Int24);
                    }

                case "int":
                    if (column.Type.Properties.SafeGetValue("unsigned") == "true")
                    {
                        return new MySqlParameter(parameterName, MySqlDbType.UInt32);
                    }
                    else
                    {
                        return new MySqlParameter(parameterName, MySqlDbType.Int32);
                    }

                case "bigint":
                    if (column.Type.Properties.SafeGetValue("unsigned") == "true")
                    {
                        return new MySqlParameter(parameterName, MySqlDbType.UInt64);
                    }
                    else
                    {
                        return new MySqlParameter(parameterName, MySqlDbType.Int64);
                    }

                case "decimal":
                    return new MySqlParameter(parameterName, MySqlDbType.Decimal);

                case "float":
                    return new MySqlParameter(parameterName, MySqlDbType.Float);

                case "double":
                    return new MySqlParameter(parameterName, MySqlDbType.Double);

                case "char":
                    return new MySqlParameter(parameterName, MySqlDbType.String);

                case "varchar":
                    return new MySqlParameter(parameterName, MySqlDbType.VarChar);

                case "date":
                    return new MySqlParameter(parameterName, MySqlDbType.Date);

                case "datetime":
                    return new MySqlParameter(parameterName, MySqlDbType.DateTime);

                case "timestamp":
                    return new MySqlParameter(parameterName, MySqlDbType.Timestamp);

                case "time":
                    return new MySqlParameter(parameterName, MySqlDbType.Time);

                case "year":
                    return new MySqlParameter(parameterName, MySqlDbType.Year);

                case "binary":
                    return new MySqlParameter(parameterName, MySqlDbType.Binary);

                case "varbinary":
                    return new MySqlParameter(parameterName, MySqlDbType.VarBinary);

                case "tinytext":
                    return new MySqlParameter(parameterName, MySqlDbType.TinyText);

                case "text":
                    return new MySqlParameter(parameterName, MySqlDbType.Text);

                case "mediumtext":
                    return new MySqlParameter(parameterName, MySqlDbType.MediumText);

                case "longtext":
                    return new MySqlParameter(parameterName, MySqlDbType.LongText);

                case "tinyblob":
                    return new MySqlParameter(parameterName, MySqlDbType.TinyBlob);

                case "blob":
                    return new MySqlParameter(parameterName, MySqlDbType.Blob);

                case "mediumblob":
                    return new MySqlParameter(parameterName, MySqlDbType.MediumBlob);

                case "longblob":
                    return new MySqlParameter(parameterName, MySqlDbType.LongBlob);

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
