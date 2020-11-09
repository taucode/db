using System;
using TauCode.Extensions;

namespace TauCode.Db.DbValueConverters
{
    public class Int64ValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value.GetType().IsIn(
                
                typeof(byte),
                typeof(sbyte),
                typeof(short),
                typeof(ushort),
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong),
                
                typeof(bool)))
            {
                return Convert.ToInt64(value);
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is long longValue)
            {
                return longValue;
            }

            return null;
        }
    }
}
