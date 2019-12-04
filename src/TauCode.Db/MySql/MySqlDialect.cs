using System;
using TauCode.Db.Model;

namespace TauCode.Db.MySql
{
    [Dialect(
        "sql-mysql-reserved-words.txt",
        "sql-mysql-data-type-names.txt",
        "``,\"\"")]
    public class MySqlDialect : DialectBase
    {
        #region Static

        public static readonly MySqlDialect Instance = new MySqlDialect();

        #endregion

        #region Constructor

        private MySqlDialect()
            : base("MySQL")
        {
        }

        #endregion

        #region Overridden

        public override bool CanDecorateTypeIdentifier => throw new NotImplementedException();

        protected override IUtilityFactory GetFactoryImpl()
        {
            throw new NotImplementedException();
        }

        public override bool IsClauseTerminatorMandatory => throw new NotImplementedException();

        public override string ValueToSqlValueString(DbTypeMold type, object value)
        {
            throw new NotImplementedException(); // todo: need this at all?

            //if (type == null)
            //{
            //    throw new ArgumentNullException(nameof(type));
            //}

            //if (value == null)
            //{
            //    return base.ValueToSqlValueString(type, value);
            //}

            //string literal;

            //var family = this.GetTypeFamily(type.Name);
            //var typeName = type.Name.ToLower();

            //if (family == DbTypeFamily.UnicodeText &&
            //    typeName == "char" &&
            //    value is Guid guid)
            //{
            //    literal = $"'{guid:N}'";
            //}
            //else if (family == DbTypeFamily.FloatingPointNumber)
            //{
            //    if (typeName == "float")
            //    {
            //        if (value is float floatValue)
            //        {
            //            literal = floatValue.ToString(CultureInfo.InvariantCulture);
            //        }
            //        else
            //        {
            //            throw new ArgumentException($"Expected value type is 'float'.", nameof(value));
            //        }
            //    }
            //    else if (typeName == "double")
            //    {
            //        if (value is double doubleValue)
            //        {
            //            literal = doubleValue.ToString(CultureInfo.InvariantCulture);
            //        }
            //        else
            //        {
            //            throw new ArgumentException($"Expected value type is 'double'.", nameof(value));
            //        }
            //    }
            //    else
            //    {
            //        throw new ArgumentException($"Unrecognized value for '{family}' type family", nameof(type));
            //    }
            //}
            //else if (family == DbTypeFamily.DateTime)
            //{
            //    if (typeName == "datetime")
            //    {
            //        if (value is DateTime dateTime)
            //        {
            //            literal = $"'{dateTime:yyyy-MM-ddTHH:mm:ss}'";
            //        }
            //        else
            //        {
            //            throw new ArgumentException($"Expected value type is '{typeof(DateTime).FullName}'.", nameof(value));
            //        }
            //    }
            //    else if (typeName == "date")
            //    {
            //        if (value is DateTime dateTime)
            //        {
            //            literal = $"'{dateTime:yyyy-MM-dd}'";
            //        }
            //        else
            //        {
            //            throw new ArgumentException($"Expected value type is '{typeof(DateTime).FullName}'.", nameof(value));
            //        }
            //    }
            //    else
            //    {
            //        literal = base.ValueToSqlValueString(type, value);
            //    }
            //}
            //else
            //{
            //    literal = base.ValueToSqlValueString(type, value);
            //}

            //return literal;
        }

        #endregion
    }
}
