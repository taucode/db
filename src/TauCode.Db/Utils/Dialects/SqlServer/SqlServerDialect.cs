using System;
using TauCode.Db.Model;

namespace TauCode.Db.Utils.Dialects.SqlServer
{
    [Dialect(
        "sql-sqlserver-reserved-words.txt",
        "sql-sqlserver-data-type-names.txt",
        "[],\"\"")]
    public class SqlServerDialect : DialectBase
    {
        #region Static

        public static readonly SqlServerDialect Instance = new SqlServerDialect();

        #endregion

        #region Constructor

        private SqlServerDialect()
            : base("SQL Server")
        {
        }

        #endregion

        #region Overridden

        public override string ValueToSqlValueString(DbTypeMold type, object value)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (value == null)
            {
                return base.ValueToSqlValueString(type, value);
            }

            var family = this.GetTypeFamily(type.Name);
            string literal;
            var typeName = type.Name.ToLower();

            if (family == DbTypeFamily.DateTime && 
                typeName == "date" && 
                value is DateTime date)
            {
                literal = $"'{date:yyyy-MM-dd}'";
            }
            else
            {
                literal = base.ValueToSqlValueString(type, value);
            }

            return literal;
        }

        public override string UnicodeTextLiteralPrefix => "N";

        #endregion
    }
}
