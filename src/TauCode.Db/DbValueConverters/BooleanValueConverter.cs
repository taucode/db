namespace TauCode.Db.DbValueConverters
{
    public class BooleanValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is bool booValue)
            {
                return booValue;
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is bool boolDbValue)
            {
                return boolDbValue;
            }

            return null;
        }
    }
}
