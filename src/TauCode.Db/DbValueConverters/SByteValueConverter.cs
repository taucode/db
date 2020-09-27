namespace TauCode.Db.DbValueConverters
{
    public class SByteValueConverter : IDbValueConverter
    {
        public object ToDbValue(object value)
        {
            if (value is long longValue)
            {
                return (sbyte)longValue;
            }

            return null;
        }

        public object FromDbValue(object dbValue)
        {
            if (dbValue is sbyte sbyteValue)
            {
                return sbyteValue;
            }

            return null;
        }
    }
}
