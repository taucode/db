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
                    $"Cannot convert instance of '{typeof(DBNull).FullName}'. For row column values, use CLR null instead of '{typeof(DBNull).FullName}'.");
            }

            var dbValue = this.ToDbValueImpl(value);

            return dbValue;
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
