using MySql.Data.MySqlClient;
using System.Data;

namespace TauCode.Db.MySql
{
    public class MySqlUtilityFactory : IDbUtilityFactory
    {
        public static MySqlUtilityFactory Instance { get; } = new MySqlUtilityFactory();

        private MySqlUtilityFactory()
        {
        }

        public IDbDialect GetDialect() => MySqlDialect.Instance;

        public IDbScriptBuilder CreateScriptBuilder(string schemaName) => new MySqlScriptBuilder(schemaName);

        public IDbConnection CreateConnection() => new MySqlConnection();
        public IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection)
        {
            return new MySqlSchemaExplorer((MySqlConnection)connection);
        }

        public IDbInspector CreateInspector(IDbConnection connection, string schemaName)
        {
            return new MySqlInspector((MySqlConnection)connection);
        }

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string schemaName, string tableName)
        {
            return new MySqlTableInspector((MySqlConnection)connection, tableName);
        }

        public IDbCruder CreateCruder(IDbConnection connection, string schemaName)
        {
            return new MySqlCruder((MySqlConnection)connection);
        }

        public IDbSerializer CreateSerializer(IDbConnection connection, string schemaName)
        {
            return new MySqlSerializer((MySqlConnection)connection);
        }
    }
}
