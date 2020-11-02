using System.Collections.Generic;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.Npgsql
{
    [DbDialect(
        typeof(NpgsqlDialectLab),
        "reserved-words.txt",
        "\"\"")]
    public class NpgsqlDialectLab : DbDialectBase
    {
        #region Static

        public static readonly NpgsqlDialectLab Instance = new NpgsqlDialectLab();

        #endregion

        private NpgsqlDialectLab()
            : base(DbProviderNames.PostgreSQL)
        {
        }

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactoryLab.Instance;

        public override bool CanDecorateTypeIdentifier => false;

        public override IList<IndexMold> GetCreatableIndexes(TableMold tableMold)
        {
            var pk = tableMold.PrimaryKey;

            return base.GetCreatableIndexes(tableMold)
                .Where(x => x.Name != pk?.Name)
                .ToList();
        }
    }
}
