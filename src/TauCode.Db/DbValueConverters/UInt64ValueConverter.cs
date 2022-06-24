using System;

namespace TauCode.Db.DbValueConverters
{
    public class UInt64ValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value.GetType().IsIntegerType())
            {
                return Convert.ToUInt64(value);
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
