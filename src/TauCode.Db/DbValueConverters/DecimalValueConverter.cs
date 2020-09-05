namespace TauCode.Db.DbValueConverters
{
    public class DecimalValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is decimal decimalValue)
            {
                return decimalValue;
            }
            else if (value is double doubleValue)
            {
                return doubleValue;
            }
            else if (value is int intValue)
            {
                return intValue;
            }
            else if (value is long longValue)
            {
                return longValue;
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is decimal decimalDbValue)
            {
                return decimalDbValue;
            }

            return null;
        }
    }
}
