using MySql.Data.MySqlClient;
using System;
using System.Data;
using TauCode.Db;
using TauCode.Db.Data;
using TauCode.Db.Model;
using TauCode.Db.Schema;

// todo everything
namespace TauCode.Lab.Db.MySql
{
    public class MySqlJsonMigrator : DbJsonMigratorBase
    {
        public MySqlJsonMigrator(
            MySqlConnection connection,
            Func<string> metadataJsonGetter,
            Func<string> dataJsonGetter,
            Func<string, bool> tableNamePredicate = null,
            Func<TableMold, DynamicRow, DynamicRow> rowTransformer = null)
            : base(
                connection,
                connection?.Database,
                metadataJsonGetter,
                dataJsonGetter,
                tableNamePredicate,
                rowTransformer)
        {
        }

        protected MySqlConnection MySqlConnection => (MySqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => MySqlUtilityFactory.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection)
        {
            return new MySqlSchemaExplorer(this.MySqlConnection);
        }
    }
}
