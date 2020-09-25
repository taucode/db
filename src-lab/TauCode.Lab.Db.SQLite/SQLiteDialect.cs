using TauCode.Db;

// todo clean
namespace TauCode.Lab.Db.SQLite
{
    [DbDialect(
        typeof(SQLiteDialect),
        "reserved-words.txt",
        //"data-type-names.txt",
        "[],\"\",``")]
    public class SQLiteDialect : DbDialectBase
    {
        #region Static

        public static readonly SQLiteDialect Instance = new SQLiteDialect();
        private static readonly string[] EmptyStrings = { };

        #endregion

        #region Constructor

        private SQLiteDialect()
            : base(DbProviderNames.SQLite)
        {
        }

        #endregion

        #region Overridden

        public override IDbUtilityFactory Factory => SQLiteUtilityFactory.Instance;

        public override bool CanDecorateTypeIdentifier => false;

        //public override IReadOnlyList<string> DataTypeNames => EmptyStrings;

        //public override bool IsSingleWordTypeName(string typeName)
        //{
        //    if (typeName == null)
        //    {
        //        throw new ArgumentNullException(nameof(typeName));
        //    }

        //    return false;
        //}

        //public override bool IsSizedTypeName(string typeName)
        //{
        //    if (typeName == null)
        //    {
        //        throw new ArgumentNullException(nameof(typeName));
        //    }

        //    return false;
        //}

        //public override bool IsPreciseNumberTypeName(string typeName)
        //{
        //    if (typeName == null)
        //    {
        //        throw new ArgumentNullException(nameof(typeName));
        //    }

        //    return false;
        //}

        //public override DbTypeFamily GetTypeFamily(string typeName)
        //{
        //    if (typeName == null)
        //    {
        //        throw new ArgumentNullException(nameof(typeName));
        //    }

        //    return DbTypeFamily.Unknown;
        //}

        //public override DbTypeNameCategory GetTypeNameCategory(string typeName)
        //{
        //    if (typeName == null)
        //    {
        //        throw new ArgumentNullException(nameof(typeName));
        //    }

        //    return DbTypeNameCategory.Unknown;
        //}

        #endregion
    }
}
