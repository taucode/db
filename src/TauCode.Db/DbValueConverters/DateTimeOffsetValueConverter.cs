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

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            throw new NotImplementedException();
        }
    }
}
