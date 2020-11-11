using System;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql.DbValueConverters
{
    public class MySqlTinyIntValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value.GetType().IsIntegerType() || value is bool)
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

            if (dbValue is bool boolValue)
            {
                return Convert.ToSByte(boolValue);
            }

            return null;
        }
    }
}
