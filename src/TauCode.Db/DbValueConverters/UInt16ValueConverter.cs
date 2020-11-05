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
            else if (value is long longValue)
            {
                checked
                {
                    return (ushort)longValue;
                }
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
