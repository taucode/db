using System;

namespace TauCode.Db.DbValueConverters
{
    public class DateTimeOffsetValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset;
            }

            if (value is DateTime dateTime)
            {
                DateTimeOffset dateTimeOffset2 = dateTime;
                dateTimeOffset2 = dateTimeOffset2.ToUniversalTime();
                return dateTimeOffset2;
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
                return dateTimeOffset2;
            }

            return null;
        }
    }
}
