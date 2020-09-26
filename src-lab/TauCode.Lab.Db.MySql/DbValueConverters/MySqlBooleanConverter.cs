using System;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql.DbValueConverters
{
    public class MySqlBooleanConverter : IDbValueConverter
    {
        public object ToDbValue(object value)
        {
            if (value is bool b)
            {
                return Convert.ToSByte(b);
            }

            return null;
        }

        public object FromDbValue(object dbValue)
        {
            if (dbValue is bool b)
            {
                return b;
            }

            return null;
        }
    }
}
