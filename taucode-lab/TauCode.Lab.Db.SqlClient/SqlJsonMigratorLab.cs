using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Data;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlJsonMigratorLab : DbJsonMigratorBase
    {
        public SqlJsonMigratorLab(
            SqlConnection connection,
            string schemaName,
            Func<string> metadataJsonGetter,
            Func<string> dataJsonGetter,
            Func<string, bool> tableNamePredicate = null,
            Func<TableMold, DynamicRow, DynamicRow> rowTransformer = null)
            : base(
                connection,
                schemaName ?? SqlToolsLab.DefaultSchemaName,
                metadataJsonGetter,
                dataJsonGetter,
                tableNamePredicate,
                rowTransformer)
        {
        }

        public override IDbUtilityFactory Factory => SqlUtilityFactoryLab.Instance;

        protected override IList<IndexMold> GetMigratableIndexes(TableMold table)
        {
            var pkIndex = table.Indexes.SingleOrDefault(x => x.Name == table.PrimaryKey?.Name);
            return table.Indexes
                .Where(x => x.Name != pkIndex?.Name)
                .ToList();
        }
    }
}
