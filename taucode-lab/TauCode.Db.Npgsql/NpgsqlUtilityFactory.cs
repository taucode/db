using Npgsql;
using System.Data;

namespace TauCode.Db.Npgsql
{
    public class NpgsqlUtilityFactory : IDbUtilityFactory
    {
        public static NpgsqlUtilityFactory Instance { get; } = new NpgsqlUtilityFactory();

        private NpgsqlUtilityFactory()
        {
        }

        public IDbDialect GetDialect() => NpgsqlDialect.Instance;

        public IDbScriptBuilder CreateScriptBuilder(string schemaName) => new NpgsqlScriptBuilder(schemaName);

        public IDbConnection CreateConnection() => new NpgsqlConnection();

        public IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection)
        {
            return new NpgsqlSchemaExplorer((NpgsqlConnection)connection);
        }

        public IDbInspector CreateInspector(IDbConnection connection, string schemaName) =>
            new NpgsqlInspector((NpgsqlConnection)connection, schemaName);

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string schemaName, string tableName) =>
            new NpgsqlTableInspector((NpgsqlConnection)connection, schemaName, tableName);

        public IDbCruder CreateCruder(IDbConnection connection, string schemaName) =>
            new NpgsqlCruder((NpgsqlConnection)connection, schemaName);

        public IDbSerializer CreateSerializer(IDbConnection connection, string schemaName) =>
            new NpgsqlSerializer((NpgsqlConnection)connection, schemaName);
    }
}
