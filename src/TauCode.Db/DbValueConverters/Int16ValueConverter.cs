using System;

namespace TauCode.Db.DbValueConverters
{
    public class Int16ValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is short shortValue)
            {
                return shortValue;
            }
            else if (value is long longValue)
            {
                return longValue;
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            throw new NotImplementedException();
        }
    }
}
