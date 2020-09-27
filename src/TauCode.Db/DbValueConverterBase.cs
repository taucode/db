using System;

namespace TauCode.Db
{
    public abstract class DbValueConverterBase : IDbValueConverter
    {
        protected abstract object ToDbValueImpl(object value);
        protected abstract object FromDbValueImpl(object dbValue);

        public object ToDbValue(object value)
        {
            if (value == null)
            {
                return DBNull.Value;
            }

            if (value == DBNull.Value)
            {
                throw new ArgumentException(
                    "Cannot convert 'DBNull.Value' to DB value. Should be 'null' instead of 'DBNull.Value'.");
            }

            return this.ToDbValueImpl(value);
        }

        public object FromDbValue(object dbValue)
        {
            if (dbValue == null)
            {
                throw new ArgumentNullException(nameof(dbValue), "DB provider should not deliver 'null'.");
            }

            if (dbValue == DBNull.Value)
            {
                return null;
            }

            return this.FromDbValueImpl(dbValue);
        }
    }
}
