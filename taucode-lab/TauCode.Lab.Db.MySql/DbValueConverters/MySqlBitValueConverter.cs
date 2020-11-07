using TauCode.Db;

namespace TauCode.Lab.Db.MySql.DbValueConverters
{
    public class MySqlBitValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is int intValue)
            {
                return intValue;
            }

            if (value is ulong ulongValue)
            {
                return ulongValue;
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is ulong ulongValue)
            {
                return ulongValue;
            }

            return null;
        }
    }
}
