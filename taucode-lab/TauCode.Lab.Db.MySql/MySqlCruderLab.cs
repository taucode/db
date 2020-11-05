using MySql.Data.MySqlClient;
using System;
using System.Data;
using TauCode.Db;
using TauCode.Db.DbValueConverters;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlCruderLab : DbCruderBase
    {
        public MySqlCruderLab(MySqlConnection connection)
            : base(connection, connection.GetSchemaName())
        {
        }

        public override IDbUtilityFactory Factory => MySqlUtilityFactoryLab.Instance;

        protected override IDbValueConverter CreateDbValueConverter(ColumnMold column)
        {
            switch (column.Type.Name)
            {
                case "tinyint":
                    if (column.Type.Properties.SafeGetValue("unsigned") == "true")
                    {
                        return new ByteValueConverter();
                    }
                    else
                    {
                        return new SByteValueConverter();
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

                case "tinytext":
                case "text":
                case "mediumtext":
                case "longtext":
                case "char":
                case "varchar":
                    return new StringValueConverter();

                default:
                    throw new NotImplementedException();
            }
        }

        protected override IDbDataParameter CreateParameter(string tableName, ColumnMold column)
        {
            const string parameterName = "parameter_name_placeholder";

            switch (column.Type.Name)
            {
                case "int":
                    if (column.Type.Properties.SafeGetValue("unsigned") == "true")
                    {
                        return new MySqlParameter(parameterName, MySqlDbType.UInt32);
                    }
                    else
                    {
                        return new MySqlParameter(parameterName, MySqlDbType.Int32);
                    }

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
