using System.Globalization;
using TauCode.Db.Model.Interfaces;
using TauCode.Extensions;

namespace TauCode.Db;

public class DefaultValueConverter : IValueConverter
{
    public object ConvertFromDb(string? schemaName, string tableName, IColumnMold columnMold, object value)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        throw new NotImplementedException();
    }

    public object ConvertToDb(string? schemaName, string tableName, IColumnMold columnMold, object? value)
    {
        if (value == null)
        {
            return DBNull.Value;
        }

        var result = this.ConvertToDbImpl(schemaName, tableName, columnMold, value);

        if (result == null)
        {
            throw new NotImplementedException(); // must not be null
        }

        return result;
    }

    protected virtual object ConvertToDbImpl(string? schemaName, string tableName, IColumnMold columnMold, object value)
    {
        if (columnMold.Type.Name.IsIn("int", "bigint", "smallint"))
        {
            if (value is
                int or
                long or
                short or
                byte)
            {
                return value;
            }

            throw new NotImplementedException();
        }

        if (columnMold.Type.Name == "bit")
        {
            if (value is bool b)
            {
                return b;
            }

            throw new NotImplementedException();
        }

        if (columnMold.Type.Name == "uniqueidentifier")
        {
            if (value is Guid)
            {
                return value;
            }

            if (value is string s)
            {
                var parsedGuid = Guid.TryParse(s, out var guid);
                if (parsedGuid)
                {
                    return guid;
                }

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        if (columnMold.Type.Name == "varchar")
        {
            if (value is string s)
            {
                return s;
            }

            throw new NotImplementedException();
        }

        if (columnMold.Type.Name == "datetimeoffset")
        {
            if (value is DateTimeOffset dateTimeOffset)
            {
                return dateTimeOffset;
            }

            if (value is string s)
            {
                var parsed = DateTimeOffset.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTimeOffset);
                if (parsed)
                {
                    return dateTimeOffset;
                }

                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }

        if (columnMold.Type.Name == "decimal")
        {
            if (value is double doubleValue)
            {
                return (decimal)doubleValue;
            }

            if (value is decimal decimalValue)
            {
                return decimalValue;
            }

            throw new NotImplementedException();
        }

        throw new NotImplementedException();
    }
}
