using System;

namespace TauCode.Db.DbValueConverters
{
    public class DateTimeOffsetValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset.ToUniversalTime();
            }

            if (value is DateTime dateTime)
            {
                DateTimeOffset dateTimeOffset2 = dateTime;
                dateTimeOffset2 = dateTimeOffset2.ToUniversalTime();
                return dateTimeOffset2;
            }

            if (value is string s)
            {
                var parsed = DateTime.TryParse(s, out var dateTimeFromString);

                if (parsed)
                {
                    DateTimeOffset dateTimeOffset3 = dateTimeFromString;
                    return dateTimeOffset3;
                }
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset;
            }

            if (dbValue is DateTime dateTime)
            {
                DateTimeOffset dateTimeOffset2 = dateTime;
                dateTimeOffset2 = dateTimeOffset2.ToUniversalTime();
                return dateTimeOffset2;
            }

            return null;
        }
    }
}
