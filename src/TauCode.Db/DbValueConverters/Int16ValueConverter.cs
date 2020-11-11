using System;

namespace TauCode.Db.DbValueConverters
{
    public class Int16ValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value.GetType().IsIntegerType())
            {
                return Convert.ToInt16(value);
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
