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
            else if (value is string s)
            {
                var parsed = Guid.TryParse(s, out guid);
                if (parsed)
                {
                    return guid;
                }
            }

            return null;
        }

        public object FromDbValue(object dbValue)
        {
            if (dbValue is Guid guid)
            {
                return guid;
            }
            else if (dbValue is string s)
            {
                var parsed = Guid.TryParse(s, out guid);
                if (parsed)
                {
                    return guid;
                }
            }

            return null;
        }
    }
}
