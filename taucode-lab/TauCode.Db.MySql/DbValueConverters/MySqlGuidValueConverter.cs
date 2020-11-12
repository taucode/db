using System;

namespace TauCode.Db.MySql.DbValueConverters
{
    public class MySqlGuidValueConverter : DbValueConverterBase
    {
        public MySqlGuidValueConverter(MySqlGuidBehaviour behaviour)
        {
            this.Behaviour = behaviour;
        }

        public MySqlGuidBehaviour Behaviour { get; }

        protected override object ToDbValueImpl(object value)
        {
            Guid? guid = null;

            if (value is Guid guid1)
            {
                guid = guid1;
            }
            else if (value is string stringValue)
            {
                guid = new Guid(stringValue);
            }
            else if (value is byte[] bytes)
            {
                guid = new Guid(bytes);
            }

            if (guid.HasValue)
            {
                switch (this.Behaviour)
                {
                    case MySqlGuidBehaviour.Binary:
                        return guid.Value.ToByteArray();

                    case MySqlGuidBehaviour.Char32:
                        return guid.Value.ToString("N");

                    case MySqlGuidBehaviour.Char36:
                        return guid.Value.ToString();
                }
            }

            return null;
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is Guid guidDbValue)
            {
                return guidDbValue;
            }

            return null;
        }
    }
}
