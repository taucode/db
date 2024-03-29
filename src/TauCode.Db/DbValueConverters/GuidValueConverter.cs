﻿using System;

namespace TauCode.Db.DbValueConverters
{
    public class GuidValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is Guid guid)
            {
                return guid;
            }
            else if (value is string stringValue)
            {
                return new Guid(stringValue);
            }
            else if (value is byte[] bytes)
            {
                return new Guid(bytes);
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
