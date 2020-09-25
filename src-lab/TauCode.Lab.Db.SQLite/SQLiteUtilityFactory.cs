using System.Data;
using TauCode.Db;

// todo clean up, regions
namespace TauCode.Lab.Db.SQLite
{
    // todo: check schema param is null
    public class SQLiteUtilityFactory : IDbUtilityFactory
    {
        public static SQLiteUtilityFactory Instance { get; } = new SQLiteUtilityFactory();

        private SQLiteUtilityFactory()
        {
        }

        //public string DbProviderName => DbProviderNames.SQLite;

        // NB: after creating an SQLite connection, you'd better to execute 2 statements to improve INSERT performance:
        // PRAGMA journal_mode = WAL
        // PRAGMA synchronous = NORMAL
        // see here: https://stackoverflow.com/questions/3852068/sqlite-insert-very-slow
        //public IDbConnection CreateConnection() => DbUtils.CreateConnection(this.DbProviderName);

        public IDbDialect GetDialect() => SQLiteDialect.Instance;

        public IDbScriptBuilder CreateScriptBuilder(string schema) => new SQLiteScriptBuilder();

        public IDbInspector CreateDbInspector(IDbConnection connection, string schema) => new SQLiteInspector(connection);

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string schema, string tableName) =>
            new SQLiteTableInspector(connection, tableName);

        public IDbCruder CreateCruder(IDbConnection connection, string schema) => new SQLiteCruder(connection);

        public IDbSerializer CreateDbSerializer(IDbConnection connection, string schema) => new SQLiteSerializer(connection);
        public IDbConverter CreateDbConverter() => new SQLiteConverter();
    }
}
