using System.Collections.Generic;
using System.Linq;
using TauCode.Db.Model;

namespace TauCode.Db.MySql
{
    [DbDialect(
        typeof(MySqlDialect),
        "reserved-words.txt",
        "``")]

    public class MySqlDialect : DbDialectBase
    {
        public static readonly MySqlDialect Instance = new MySqlDialect();

        private MySqlDialect()
            : base(DbProviderNames.MySQL)
        {
        }

        public override IDbUtilityFactory Factory => MySqlUtilityFactory.Instance;

        public override bool CanDecorateTypeIdentifier => false;

        public override IList<IndexMold> GetCreatableIndexes(TableMold tableMold)
        {
            // todo: null arg ex possible

            var pk = tableMold.PrimaryKey;
            var fkNames = tableMold.ForeignKeys.Select(x => x.Name).ToHashSet();

            var indexes = new List<IndexMold>();

            foreach (var originalIndex in base.GetCreatableIndexes(tableMold))
            {
                if (originalIndex.Name == pk?.Name)
                {
                    continue;
                }

                if (fkNames.Contains(originalIndex.Name))
                {
                    continue;
                }

                indexes.Add(originalIndex);
            }

            return indexes;
        }
    }
}
