using System;

namespace TauCode.Db.DbValueConverters
{
    public class SByteValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value.GetType().IsIntegerType())
            {
                return Convert.ToSByte(value);
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is sbyte sbyteValue)
            {
                return sbyteValue;
            }

            return null;
        }
    }
}
