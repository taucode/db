using System;

namespace TauCode.Db.DbValueConverters
{
    public class Int32ValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value.GetType().IsIntegerType())
            {
                return Convert.ToInt32(value);
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is int intDbValue)
            {
                return intDbValue;
            }

            return null;
        }
    }
}
