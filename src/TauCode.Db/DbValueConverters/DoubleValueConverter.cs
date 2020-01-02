using System;

namespace TauCode.Db.DbValueConverters
{
    public class DoubleValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is double doubleValue)
            {
                return doubleValue;
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is double doubleDbValue)
            {
                return doubleDbValue;
            }

            return DBNull.Value;
        }
    }
}
