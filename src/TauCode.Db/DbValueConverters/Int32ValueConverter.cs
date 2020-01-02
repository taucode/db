namespace TauCode.Db.DbValueConverters
{
    public class Int32ValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is int intValue)
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
            if (dbValue is int intDbValue)
            {
                return intDbValue;
            }

            return null;
        }
    }
}
