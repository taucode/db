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

            if (value is string s)
            {
                var parsed = TimeSpan.TryParse(s, out var timeSpan2);
                if (parsed)
                {
                    return timeSpan2;
                }
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
