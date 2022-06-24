using System;

namespace TauCode.Db.DbValueConverters
{
    public class ByteValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value.GetType().IsIntegerType())
            {
                return Convert.ToByte(value);
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is byte byteDbValue)
            {
                return byteDbValue;
            }

            return null;
        }
    }
}
