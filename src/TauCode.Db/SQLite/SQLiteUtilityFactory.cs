using System.Data;

namespace TauCode.Db.SQLite
{
    public class SQLiteUtilityFactory : IUtilityFactory
    {
        public static SQLiteUtilityFactory Instance { get; } = new SQLiteUtilityFactory();

        private SQLiteUtilityFactory()
        {
        }

        public string DbProviderName => DbProviderNames.SQLite;

        // NB: after creating an SQLite connection, you'd better to execute 2 statements to improve INSERT performance:
        // PRAGMA journal_mode = WAL
        // PRAGMA synchronous = NORMAL
        // see here: https://stackoverflow.com/questions/3852068/sqlite-insert-very-slow
        public IDbConnection CreateConnection() => DbUtils.CreateConnection(this.DbProviderName);

        public IDialect GetDialect() => SQLiteDialect.Instance;

        public IScriptBuilder CreateScriptBuilder() => new SQLiteScriptBuilder();

        public IDbInspector CreateDbInspector(IDbConnection connection) => new SQLiteInspector(connection);

        public ITableInspector CreateTableInspector(IDbConnection connection, string tableName) =>
            new SQLiteTableInspector(connection, tableName);

        public ICruder CreateCruder(IDbConnection connection) => new SQLiteCruder(connection);

        public IDbSerializer CreateDbSerializer(IDbConnection connection) => new SQLiteSerializer(connection);
    }
}
