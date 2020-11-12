using System.Data;
using System.Data.SQLite;

namespace TauCode.Db.SQLite
{
    public class SQLiteUtilityFactory : IDbUtilityFactory
    {
        public static SQLiteUtilityFactory Instance { get; } = new SQLiteUtilityFactory();

        private SQLiteUtilityFactory()
        {
        }

        public IDbDialect GetDialect() => SQLiteDialect.Instance;

        public IDbScriptBuilder CreateScriptBuilder(string schemaName)
        {
            return new SQLiteScriptBuilder();
        }

        public IDbConnection CreateConnection() => new SQLiteConnection();

        public IDbInspector CreateInspector(IDbConnection connection, string schemaName)
        {
            return new SQLiteInspector((SQLiteConnection)connection);
        }

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string schemaName, string tableName)
        {
            // todo: check schema is null, here & anywhere.
            return new SQLiteTableInspector((SQLiteConnection)connection, tableName);
        }

        public IDbCruder CreateCruder(IDbConnection connection, string schemaName)
        {
            return new SQLiteCruder((SQLiteConnection)connection);
        }

        public IDbSerializer CreateSerializer(IDbConnection connection, string schemaName)
        {
            return new SQLiteSerializer((SQLiteConnection)connection);
        }
    }
}
