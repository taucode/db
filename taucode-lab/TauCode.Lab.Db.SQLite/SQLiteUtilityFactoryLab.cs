using System.Data;
using System.Data.SQLite;
using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteUtilityFactoryLab : IDbUtilityFactory
    {
        public static SQLiteUtilityFactoryLab Instance { get; } = new SQLiteUtilityFactoryLab();

        private SQLiteUtilityFactoryLab()
        {
        }

        public IDbDialect GetDialect() => SQLiteDialectLab.Instance;

        public IDbScriptBuilder CreateScriptBuilder(string schemaName)
        {
            return new SQLiteScriptBuilderLab();
        }

        public IDbConnection CreateConnection() => new SQLiteConnection();

        public IDbInspector CreateInspector(IDbConnection connection, string schemaName)
        {
            return new SQLiteInspectorLab((SQLiteConnection)connection);
        }

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string schemaName, string tableName)
        {
            // todo: check schema is null, here & anywhere.
            return new SQLiteTableInspectorLab((SQLiteConnection)connection, tableName);
        }

        public IDbCruder CreateCruder(IDbConnection connection, string schemaName)
        {
            return new SQLiteCruderLab((SQLiteConnection)connection);
        }

        public IDbSerializer CreateSerializer(IDbConnection connection, string schemaName)
        {
            return new SQLiteSerializerLab((SQLiteConnection)connection);
        }
    }
}
