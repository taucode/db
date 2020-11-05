namespace TauCode.Db.DbValueConverters
{
    public class UInt64ValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is ulong ulongValue)
            {
                return ulongValue;
            }
            else if (value is long longValue)
            {
                checked
                {
                    return (ulong)longValue;
                }
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is long longValue)
            {
                return longValue;
            }

            return null;
        }
    }
}
