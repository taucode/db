using System;

namespace TauCode.Db.DbValueConverters
{
    public class UInt16ValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value.GetType().IsIntegerType())
            {
                return Convert.ToUInt16(value);
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
