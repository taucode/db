using System;

namespace TauCode.Db.DbValueConverters
{
    public class Int64ValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value.GetType().IsIntegerType())
            {
                return Convert.ToInt64(value);
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is long longValue)
            {
                return longValue;
            }

            return null;
        }
    }
}
