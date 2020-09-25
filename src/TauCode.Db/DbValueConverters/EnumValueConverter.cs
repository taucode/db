using System;
using System.Data;

namespace TauCode.Db.DbValueConverters
{
    // todo rearrange and make look nice
    public class EnumValueConverter<TEnum> : DbValueConverterBase
        where TEnum : struct
    {
        public EnumValueConverter(DbType dbType)
        {
            CheckGenericArg();

            this.DbType = dbType; // todo: check.
            //this.Behaviour = EnumValueConverterBehaviour.Integer;
        }

        private static void CheckGenericArg()
        {
            bool validGenericArg = typeof(TEnum).IsEnum;
            if (!validGenericArg)
            {
                throw new ArgumentException($"'{nameof(TEnum)}' must be an enum type.", nameof(TEnum));
            }
        }

        //public EnumValueConverter(EnumValueConverterBehaviour behaviour)
        //{
        //    this.Behaviour = behaviour;
        //}

        public DbType DbType { get; }

        //public DbType 

        //public EnumValueConverterBehaviour Behaviour { get; }

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
                    if (value is TEnum || DbTools.IsIntegerType(value.GetType()))
                    {
                        return Convert.ToByte(value);
                    }

                    break;

                case DbType.SByte:
                    if (value is TEnum || DbTools.IsIntegerType(value.GetType()))
                    {
                        return Convert.ToSByte(value);
                    }

                    break;

                case DbType.Int16:
                    if (value is TEnum || DbTools.IsIntegerType(value.GetType()))
                    {
                        return Convert.ToInt16(value);
                    }

                    break;

                case DbType.UInt16:
                    if (value is TEnum || DbTools.IsIntegerType(value.GetType()))
                    {
                        return Convert.ToUInt16(value);
                    }

                    break;

                case DbType.Int32:
                    if (value is TEnum || DbTools.IsIntegerType(value.GetType()))
                    {
                        return Convert.ToInt32(value);
                    }

                    break;

                case DbType.UInt32:
                    if (value is TEnum || DbTools.IsIntegerType(value.GetType()))
                    {
                        return Convert.ToUInt32(value);
                    }

                    break;

                case DbType.Int64:
                    if (value is TEnum || DbTools.IsIntegerType(value.GetType()))
                    {
                        return Convert.ToInt64(value);
                    }

                    break;

                case DbType.UInt64:
                    if (value is TEnum || DbTools.IsIntegerType(value.GetType()))
                    {
                        return Convert.ToUInt64(value);
                    }

                    break;

                default:
                    throw new ArgumentOutOfRangeException(); // todo
            }

            //switch (this.Behaviour)
            //{
            //    case EnumValueConverterBehaviour.Integer:
            //        if (DbTools.IsIntegerType(value.GetType()))
            //        {
            //            return value;
            //        }
            //        break;

            //    case EnumValueConverterBehaviour.String:
            //        if (value is TEnum)
            //        {
            //            return value.ToString();
            //        }
            //        break;
            //}

            return null;
        }

     

        protected override object FromDbValueImpl(object dbValue)
        {
            throw new NotImplementedException();
            //switch (this.Behaviour)
            //{
            //    case EnumValueConverterBehaviour.Integer:
            //        return this.FromDbValueAsInteger(dbValue);

            //    case EnumValueConverterBehaviour.String:
            //        return this.FromDbValueAsString(dbValue);

            //    default:
            //        throw new ArgumentOutOfRangeException();
            //}
        }

        protected virtual object FromDbValueAsInteger(object dbValue)
        {
            throw new NotImplementedException();
        }

        protected virtual TEnum? FromDbValueAsString(object dbValue)
        {
            if (dbValue is string stringDbValue)
            {
                var parsed = Enum.TryParse<TEnum>(stringDbValue, out var result);
                if (parsed)
                {
                    return result;
                }
            }

            return null;
        }
    }
}
