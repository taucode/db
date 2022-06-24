using System;

namespace TauCode.Db.DbValueConverters
{
    public class UInt32ValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value.GetType().IsIntegerType())
            {
                return Convert.ToUInt32(value);
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
