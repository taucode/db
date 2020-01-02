using System;

namespace TauCode.Db.DbValueConverters
{
    public class ByteValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is byte byteValue)
            {
                return byteValue;
            }
            else if (value is long longValue)
            {
                return longValue;
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is byte byteDbValue)
            {
                return byteDbValue;
            }

            return DBNull.Value;
        }
    }
}
