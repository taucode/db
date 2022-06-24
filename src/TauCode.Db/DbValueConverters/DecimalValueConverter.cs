using System;

namespace TauCode.Db.DbValueConverters
{
    public class DecimalValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value.GetType().IsNumericType())
            {
                return Convert.ToDecimal(value);
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is decimal decimalDbValue)
            {
                return decimalDbValue;
            }

            return null;
        }
    }
}
