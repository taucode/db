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
