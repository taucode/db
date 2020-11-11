using System;

namespace TauCode.Db.DbValueConverters
{
    public class SingleValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value.GetType().IsNumericType())
            {
                return Convert.ToSingle(value);
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is float floatDbValue)
            {
                return floatDbValue;
            }

            return null;
        }
    }
}
