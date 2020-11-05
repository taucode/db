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
            else if (value is long longValue)
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
            if (dbValue is int uintDbValue)
            {
                return uintDbValue;
            }

            return null;
        }
    }
}
