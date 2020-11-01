using System.Collections.Generic;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.SqlClient
{
    [DbDialect(
        typeof(SqlDialectLab),
        "reserved-words.txt",
        "[],\"\"")]
    public class SqlDialectLab : DbDialectBase
    {
        #region Static

        public static readonly SqlDialectLab Instance = new SqlDialectLab();

        #endregion

        #region Constructor

        private SqlDialectLab()
            : base(DbProviderNames.SQLServer)
        {
        }

        #endregion

        #region Overridden

        public override IDbUtilityFactory Factory => SqlUtilityFactoryLab.Instance;
        
        public override string UnicodeTextLiteralPrefix => "N";

        public override IList<IndexMold> GetCreatableIndexes(TableMold tableMold)
        {
            var pk = tableMold.PrimaryKey;

            return base.GetCreatableIndexes(tableMold)
                .Where(x => x.Name != pk?.Name)
                .ToList();
        }

        #endregion
    }
}
