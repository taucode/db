﻿using System;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql.DbValueConverters
{
    public class MySqlTinyIntValueConverter : DbValueConverterBase
    {
        protected override object ToDbValueImpl(object value)
        {
            if (value is sbyte sbyteValue)
            {
                return sbyteValue;
            }

            if (value is bool boolValue)
            {
                return Convert.ToSByte(boolValue);
            }

            if (value is byte byteValue)
            {
                return Convert.ToSByte(byteValue);
            }

            if (value is int intValue)
            {
                return Convert.ToSByte(intValue);
            }

            if (value is long longValue)
            {
                return Convert.ToSByte(longValue);
            }

            throw new NotImplementedException();
        }

        protected override object FromDbValueImpl(object dbValue)
        {
            if (dbValue is sbyte sbyteValue)
            {
                return sbyteValue;
            }

            if (dbValue is bool boolValue)
            {
                return Convert.ToSByte(boolValue);
            }

            return null;
        }
    }
}
