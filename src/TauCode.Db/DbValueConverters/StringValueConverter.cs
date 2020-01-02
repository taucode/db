using System;

namespace TauCode.Db.DbValueConverters
{
    public class StringValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is string stringValue)
            {
                return stringValue;
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is string stringDbValue)
            {
                return stringDbValue;
            }

            return DBNull.Value;
        }
    }
}
