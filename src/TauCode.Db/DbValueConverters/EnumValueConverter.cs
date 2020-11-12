using System;
using System.Data;

namespace TauCode.Db.DbValueConverters
{
    public class EnumValueConverter<TEnum> : DbValueConverterBase
        where TEnum : struct
    {
        public EnumValueConverter(DbType dbType)
        {
            CheckGenericArg();

            this.DbType = dbType;
        }

        private static void CheckGenericArg()
        {
            var validGenericArg = typeof(TEnum).IsEnum;
            if (!validGenericArg)
            {
                throw new ArgumentException($"'{nameof(TEnum)}' must be an enum type.", nameof(TEnum));
            }
        }

        public DbType DbType { get; }

        protected override object ToDbValueImpl(object value)
        {
            switch (this.DbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    if (value is TEnum)
                    {
                        return value.ToString();
                    }

                    break;

                case DbType.Byte:
                    if (value is TEnum || value.GetType().IsIntegerType())
                    {
                        return Convert.ToByte(value);
                    }

                    break;

                case DbType.SByte:
                    if (value is TEnum || value.GetType().IsIntegerType())
                    {
                        return Convert.ToSByte(value);
                    }

                    break;

                case DbType.Int16:
                    if (value is TEnum || value.GetType().IsIntegerType())
                    {
                        return Convert.ToInt16(value);
                    }

                    break;

                case DbType.UInt16:
                    if (value is TEnum || value.GetType().IsIntegerType())
                    {
                        return Convert.ToUInt16(value);
                    }

                    break;

                case DbType.Int32:
                    if (value is TEnum || value.GetType().IsIntegerType())
                    {
                        return Convert.ToInt32(value);
                    }

                    break;

                case DbType.UInt32:
                    if (value is TEnum || value.GetType().IsIntegerType())
                    {
                        return Convert.ToUInt32(value);
                    }

                    break;

                case DbType.Int64:
                    if (value is TEnum || value.GetType().IsIntegerType())
                    {
                        return Convert.ToInt64(value);
                    }

                    break;

                case DbType.UInt64:
                    if (value is TEnum || value.GetType().IsIntegerType())
                    {
                        return Convert.ToUInt64(value);
                    }

                    break;

                default:
                    throw new ArgumentException($"Unsupported type: '{this.DbType}'.", nameof(this.DbType));
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            switch (this.DbType)
            {
                case DbType.AnsiString:
                case DbType.AnsiStringFixedLength:
                case DbType.String:
                case DbType.StringFixedLength:
                    if (dbValue is string s)
                    {
                        var parsed = Enum.TryParse(typeof(TEnum), s, out var @enum);
                        if (parsed)
                        {
                            return @enum;
                        }
                    }

                    break;

                case DbType.Byte:
                case DbType.SByte:
                case DbType.Int16:
                case DbType.UInt16:
                case DbType.Int32:
                case DbType.UInt32:
                case DbType.Int64:
                case DbType.UInt64:
                    if (dbValue.GetType().IsIntegerType())
                    {
                        var @enum = Enum.ToObject(typeof(TEnum), dbValue);
                        return @enum;
                    }
                    break;

                default:
                    throw new ArgumentException($"Unsupported type: '{this.DbType}'.", nameof(this.DbType));
            }

            return null;
        }
    }
}
