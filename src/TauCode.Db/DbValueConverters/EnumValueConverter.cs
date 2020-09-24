using System;

namespace TauCode.Db.DbValueConverters
{
    public class EnumValueConverter<TEnum> : DbValueConverterBase
        where TEnum : struct
    {

        public EnumValueConverter(EnumValueConverterBehaviour behaviour)
        {
            bool validGenericArg = typeof(TEnum).IsEnum;
            if (!validGenericArg)
            {
                throw new ArgumentException($"'{nameof(TEnum)}' must be an enum type.", nameof(TEnum));
            }

            this.Behaviour = behaviour;
        }

        public EnumValueConverterBehaviour Behaviour { get; }

        protected override object ToDbValueImpl(object value)
        {
            switch (this.Behaviour)
            {
                case EnumValueConverterBehaviour.Integer:
                    if (DbTools.IsIntegerType(value.GetType()))
                    {
                        return value;
                    }
                    break;

                case EnumValueConverterBehaviour.String:
                    if (value is TEnum)
                    {
                        return value.ToString();
                    }
                    break;
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            switch (this.Behaviour)
            {
                case EnumValueConverterBehaviour.Integer:
                    return this.FromDbValueAsInteger(dbValue);

                case EnumValueConverterBehaviour.String:
                    return this.FromDbValueAsString(dbValue);

                default:
                    throw new ArgumentOutOfRangeException();
            }
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
