using System;

namespace TauCode.Db.DbValueConverters
{
    public class EnumValueConverter<TEnum> : DbValueConverterBase
        where TEnum : struct
    {
        public EnumValueConverter(EnumValueConverterBehaviour behaviour)
        {
            this.Behaviour = behaviour;
        }

        public EnumValueConverterBehaviour Behaviour { get; }

        protected override object ToDbValueImpl(object value)
        {
            throw new NotImplementedException();
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

        protected virtual TEnum FromDbValueAsString(object dbValue)
        {
            if (dbValue is string stringDbValue)
            {
                var parsed = Enum.TryParse<TEnum>(stringDbValue, out var result);
                if (parsed)
                {
                    return result;
                }

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }
    }
}
