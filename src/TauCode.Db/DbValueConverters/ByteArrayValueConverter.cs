using System;

namespace TauCode.Db.DbValueConverters
{
    public class ByteArrayValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is byte[] bytes)
            {
                return bytes;
            }
            else if (value is string base64)
            {
                return Convert.FromBase64String(base64);
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            throw new NotImplementedException();
        }
    }
}
