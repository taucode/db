using MySql.Data.MySqlClient;
using System;
using System.Data;

namespace TauCode.Db.MySql
{
    public class MySqlJsonMigrator : DbJsonMigratorBase
    {
        public MySqlJsonMigrator(
            MySqlConnection connection,
            Func<string> metadataJsonGetter,
            Func<string> dataJsonGetter,
            Func<string, bool> tableNamePredicate = null)
            : base(
                connection,
                connection?.Database,
                metadataJsonGetter,
                dataJsonGetter,
                tableNamePredicate)
        {
            MySqlTools.CheckConnectionArgument(connection);
        }

        protected MySqlConnection MySqlConnection => (MySqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => MySqlUtilityFactory.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection)
        {
            return new MySqlSchemaExplorer(this.MySqlConnection);
        }
    }
}
