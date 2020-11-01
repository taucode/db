using Microsoft.Data.SqlClient;
using System;
using TauCode.Db;
using TauCode.Db.Data;
using TauCode.Db.Model;

// todo clean up, regions, here & in other classes.
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

        protected SqlConnection SqlConnection => (SqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => SqlUtilityFactoryLab.Instance;

        protected override bool NeedCheckSchemaExistence => true;

        protected override bool SchemaExists(string schemaName) => this.SqlConnection.SchemaExists(schemaName);
    }
}
