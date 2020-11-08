namespace TauCode.Db.DbValueConverters
{
    public class UInt32ValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is uint uintValue)
            {
                return uintValue;
            }

            if (value is int intValue)
            {
                checked
                {
                    return (uint)intValue;
                }
            }

            if (value is ulong ulongValue)
            {
                checked
                {
                    return (uint)ulongValue;
                }
            }

            if (value is long longValue)
            {
                checked
                {
                    return (uint)longValue;
                }
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is uint uintDbValue)
            {
                return uintDbValue;
            }

            return null;
        }
    }
}
