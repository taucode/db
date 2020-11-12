using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db.Model;

namespace TauCode.Db.Npgsql
{
    [DbDialect(
        typeof(NpgsqlDialect),
        "reserved-words.txt",
        "\"\"")]
    public class NpgsqlDialect : DbDialectBase
    {
        #region Static

        public static readonly NpgsqlDialect Instance = new NpgsqlDialect();

        #endregion

        private NpgsqlDialect()
            : base(DbProviderNames.PostgreSQL)
        {
        }

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactory.Instance;

        public override bool CanDecorateTypeIdentifier => false;

        public override IList<IndexMold> GetCreatableIndexes(TableMold tableMold)
        {
            if (tableMold == null)
            {
                throw new ArgumentNullException(nameof(tableMold));
            }

            var pk = tableMold.PrimaryKey;

            return base.GetCreatableIndexes(tableMold)
                .Where(x => x.Name != pk?.Name)
                .ToList();
        }
    }
}
