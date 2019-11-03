using System;
using System.Collections.Generic;
using System.Linq;

namespace TauCode.Db.Utils.Dialects.SQLite
{
    [Dialect(
        "sql-sqlite-reserved-words.txt",
        "sql-sqlite-data-type-names.txt",
        "\"\",[],``")]
    public class SQLiteDialect : DialectBase
    {
        #region Static

        public static readonly SQLiteDialect Instance = new SQLiteDialect();

        #endregion

        #region Constructor

        private SQLiteDialect()
            : base("SQLite")
        {
        }

        #endregion

        #region Overridden

        public override bool CanDecorateTypeIdentifier => false;

        public override IReadOnlyList<string> DataTypeNames => throw new NotSupportedException("In SQLite, almost any word can designate type name.");

        public override bool IsSingleWordTypeName(string typeName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            return !string.IsNullOrEmpty(typeName) && !this.ReservedWords.Contains(typeName.ToUpper());
        }

        public override bool IsSizedTypeName(string typeName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            return !string.IsNullOrEmpty(typeName) && !this.ReservedWords.Contains(typeName.ToUpper());
        }

        public override bool IsPreciseNumberTypeName(string typeName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            return !string.IsNullOrEmpty(typeName) && !this.ReservedWords.Contains(typeName.ToUpper());
        }

        #endregion
    }
}
