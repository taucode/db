namespace TauCode.Db.DbValueConverters
{
    public class Int16ValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is short shortValue)
            {
                return shortValue;
            }

            if (value is int intValue)
            {
                checked
                {
                    return (short)intValue;
                }
            }

            if (value is long longValue)
            {
                checked
                {
                    return (short)longValue;
                }
            }

            if (value is byte byteValue)
            {
                return (short)byteValue;
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is short shortDbValue)
            {
                return shortDbValue;
            }

            return null;
        }
    }
}
