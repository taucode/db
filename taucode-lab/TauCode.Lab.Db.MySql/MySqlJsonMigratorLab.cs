using MySql.Data.MySqlClient;
using System;
using TauCode.Db;
using TauCode.Db.Data;
using TauCode.Db.Model;

// todo everything
namespace TauCode.Lab.Db.MySql
{
    public class MySqlJsonMigratorLab : DbJsonMigratorBase
    {
        public MySqlJsonMigratorLab(
            MySqlConnection connection,
            Func<string> metadataJsonGetter,
            Func<string> dataJsonGetter,
            Func<string, bool> tableNamePredicate = null,
            Func<TableMold, DynamicRow, DynamicRow> rowTransformer = null)
            : base(
                connection,
                connection.GetSchemaName(),
                metadataJsonGetter,
                dataJsonGetter,
                tableNamePredicate,
                rowTransformer)
        {
        }

        public override IDbUtilityFactory Factory => MySqlUtilityFactoryLab.Instance;

        protected override bool NeedCheckSchemaExistence => true;

        protected override bool SchemaExists(string schemaName) =>
            ((MySqlConnection)this.Connection).SchemaExists(schemaName);
    }
}
