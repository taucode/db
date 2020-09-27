using TauCode.Db;

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
        }

        public object FromDbValue(object dbValue)
        {
            if (dbValue is decimal decimalValue)
            {
                return decimalValue;
            }

            return null;
        }
    }
}
