using System;
using System.Data;
using Npgsql;
using TauCode.Db.Data;
using TauCode.Db.Model;
using TauCode.Db.Npgsql.Schema;
using TauCode.Db.Schema;

namespace TauCode.Db.Npgsql
{
    public class NpgsqlJsonMigrator : DbJsonMigratorBase
    {
        public NpgsqlJsonMigrator(
            NpgsqlConnection connection,
            string schemaName,
            Func<string> metadataJsonGetter,
            Func<string> dataJsonGetter,
            Func<string, bool> tableNamePredicate = null,
            Func<TableMold, DynamicRow, DynamicRow> rowTransformer = null)
            : base(
                connection,
                schemaName ?? NpgsqlTools.DefaultSchemaName,
                metadataJsonGetter,
                dataJsonGetter,
                tableNamePredicate,
                rowTransformer)
        {
        }

        protected NpgsqlConnection NpgsqlConnection => (NpgsqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactory.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection)
        {
            return new NpgsqlSchemaExplorer(this.NpgsqlConnection);
        }
    }
}
