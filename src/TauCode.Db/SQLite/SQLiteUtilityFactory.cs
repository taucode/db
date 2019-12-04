using System;
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

        public IDialect GetDialect() => SQLiteDialect.Instance;

        public IScriptBuilderLab CreateScriptBuilderLab() => /*new SQLiteScriptBuilder();*/ throw new NotImplementedException();

        public IDbInspector CreateDbInspector(IDbConnection connection) => new SQLiteInspector(connection);

        public ITableInspector CreateTableInspector(IDbConnection connection, string tableName) =>
            new SQLiteTableInspector(connection, tableName);

        public ICruder CreateCruder(IDbConnection connection) => new SQLiteCruder(connection);

        public IDbSerializer CreateDbSerializer(IDbConnection connection) => new SQLiteSerializer(connection);
    }
}
