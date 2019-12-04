using System.Data;

namespace TauCode.Db.SQLite
{
    public class SQLiteUtilityFactory : IUtilityFactory
    {
        public string DbProviderName => DbProviderNames.SQLite;

        public IDialect GetDialect() => SQLiteDialect.Instance;

        public IScriptBuilder CreateScriptBuilder() => new SQLiteScriptBuilder();

        public IDbInspector CreateDbInspector(IDbConnection connection) => new SQLiteInspector(connection);

        public ICruder CreateCruder(IDbConnection connection) => new SQLiteCruder(connection);

        public IDbSerializer CreateDbSerializer(IDbConnection connection) => new SQLiteSerializer(connection);
    }
}
