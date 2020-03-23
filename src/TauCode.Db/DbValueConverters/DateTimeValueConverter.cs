using System;

namespace TauCode.Db.DbValueConverters
{
    public class DateTimeValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime;
            }
            else if (value is string s)
            {
                var parsed = DateTime.TryParse(s, out var dateTimeFromString);
                return parsed ? (object)dateTimeFromString : null;
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is DateTime dateTimedDbValue)
            {
                return dateTimedDbValue;
            }

            return null;
        }
    }
}
