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
            
            if (value is long longValue)
            {
                checked
                {
                    return (ulong)longValue;
                }
            }

            if (value is int intValue)
            {
                checked
                {
                    return (uint)intValue;
                }
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is ulong ulongValue)
            {
                return ulongValue;
            }

            return null;
        }
    }
}
