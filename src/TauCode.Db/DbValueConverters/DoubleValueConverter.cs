using System;

namespace TauCode.Db.DbValueConverters
{
    public class DoubleValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value.GetType().IsNumericType())
            {
                return Convert.ToDouble(value);
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is double doubleDbValue)
            {
                return doubleDbValue;
            }

            return null;
        }
    }
}
