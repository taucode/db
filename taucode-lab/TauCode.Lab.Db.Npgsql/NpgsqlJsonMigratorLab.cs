using Npgsql;
using System;
using System.Data;
using TauCode.Db;
using TauCode.Db.Data;
using TauCode.Db.Model;
using TauCode.Db.Schema;

namespace TauCode.Lab.Db.Npgsql
{
    public class NpgsqlJsonMigratorLab : DbJsonMigratorBase
    {
        public NpgsqlJsonMigratorLab(
            NpgsqlConnection connection,
            string schemaName,
            Func<string> metadataJsonGetter,
            Func<string> dataJsonGetter,
            Func<string, bool> tableNamePredicate = null,
            Func<TableMold, DynamicRow, DynamicRow> rowTransformer = null)
            : base(
                connection,
                schemaName ?? NpgsqlToolsLab.DefaultSchemaName,
                metadataJsonGetter,
                dataJsonGetter,
                tableNamePredicate,
                rowTransformer)
        {
        }

        protected NpgsqlConnection NpgsqlConnection => (NpgsqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactoryLab.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection)
        {
            return new NpgsqlSchemaExplorer(this.NpgsqlConnection);
        }
    }
}
