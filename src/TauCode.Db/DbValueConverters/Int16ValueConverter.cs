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
            else if (value is long longValue)
            {
                return longValue;
            }
            else if (value is byte byteValue)
            {
                return byteValue;
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
