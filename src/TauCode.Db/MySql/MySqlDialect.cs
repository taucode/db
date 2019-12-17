using System;

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

        public override IUtilityFactory Factory => MySqlUtilityFactory.Instance;

        public override bool CanDecorateTypeIdentifier => throw new NotImplementedException();

        public override bool IsClauseTerminatorMandatory => throw new NotImplementedException();
        
        #endregion
    }
}
