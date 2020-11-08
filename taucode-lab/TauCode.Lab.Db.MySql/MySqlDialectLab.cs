using System.Collections.Generic;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.MySql
{
    [DbDialect(
        typeof(MySqlDialectLab),
        "reserved-words.txt",
        "``")]

    public class MySqlDialectLab : DbDialectBase
    {
        public static readonly MySqlDialectLab Instance = new MySqlDialectLab();

        private MySqlDialectLab()
            : base(DbProviderNames.MySQL)
        {
        }

        public override IDbUtilityFactory Factory => MySqlUtilityFactoryLab.Instance;

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
