using TauCode.Db;

// todo clean
namespace TauCode.Lab.Db.Npgsql.DbValueConverters
{
    public class NpgsqlMoneyConverter : IDbValueConverter
    {
        public object ToDbValue(object value)
        {
            if (value is double doubleValue)
            {
                var money = (decimal)doubleValue;
                return money;
            }

            return null;

            //if (value is Guid guid)
            //{
            //    return guid.ToString();
            //}
            //else if (value is string s)
            //{
            //    var parsed = Guid.TryParse(s, out guid);
            //    if (parsed)
            //    {
            //        return guid;
            //    }
            //}

            //return null;
        }

        public object FromDbValue(object dbValue)
        {
            if (dbValue is decimal decimalValue)
            {
                return decimalValue;
            }

            return null;

            //if (dbValue is Guid guid)
            //{
            //    return guid;
            //}
            //else if (dbValue is string s)
            //{
            //    var parsed = Guid.TryParse(s, out guid);
            //    if (parsed)
            //    {
            //        return guid;
            //    }
            //}

            //return null;
        }
    }
}
