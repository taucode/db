namespace TauCode.Db.DbValueConverters
{
    public class UInt16ValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is ushort ushortValue)
            {
                return ushortValue;
            }

            if (value is long longValue)
            {
                checked
                {
                    return (ushort)longValue;
                }
            }

            if (value is int intValue)
            {
                checked
                {
                    return (ushort)intValue;
                }
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is ushort ushortDbValue)
            {
                return ushortDbValue;
            }

            return null;
        }
    }
}
