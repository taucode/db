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

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            throw new NotImplementedException();
        }
    }
}
