using System;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql.DbValueConverters
{
    public class MySqlGuidConverter : IDbValueConverter
    {
        public object ToDbValue(object value)
        {
            if (value is Guid guid)
            {
                return guid.ToString();
            }

            return null;
        }

        public object FromDbValue(object dbValue)
        {
            if (dbValue is Guid guid)
            {
                return guid;
            }

            return null;
        }
    }
}
