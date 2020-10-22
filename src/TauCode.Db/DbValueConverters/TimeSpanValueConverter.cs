using System;

namespace TauCode.Db.DbValueConverters
{
    public class TimeSpanValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is TimeSpan timeSpan)
            {
                return timeSpan;
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is TimeSpan timeSpan)
            {
                return timeSpan;
            }

            return null;
        }
    }
}
