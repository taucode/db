using System;
using TauCode.Db;

// todo get rid of.
namespace TauCode.Lab.Db.MySql.DbValueConverters
{
    public class MySqlBinaryConverter : IDbValueConverter
    {
        public object ToDbValue(object value)
        {
            return new MySqlBinaryConvertible();
        }

        public object FromDbValue(object dbValue)
        {
            throw new NotImplementedException();
        }
    }
}
