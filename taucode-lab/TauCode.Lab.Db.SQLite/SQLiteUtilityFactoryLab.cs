using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteUtilityFactoryLab : IDbUtilityFactory
    {
        public static SQLiteUtilityFactoryLab Instance { get; } = new SQLiteUtilityFactoryLab();

        private SQLiteUtilityFactoryLab()
        {
        }

        public IDbDialect GetDialect()
        {
            throw new System.NotImplementedException();
        }

        public IDbScriptBuilder CreateScriptBuilder(string schemaName)
        {
            throw new System.NotImplementedException();
        }

        public IDbConnection CreateConnection()
        {
            throw new System.NotImplementedException();
        }

        public IDbInspector CreateInspector(IDbConnection connection, string schemaName)
        {
            throw new System.NotImplementedException();
        }

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string schemaName, string tableName)
        {
            throw new System.NotImplementedException();
        }

        public IDbCruder CreateCruder(IDbConnection connection, string schemaName)
        {
            throw new System.NotImplementedException();
        }

        public IDbSerializer CreateSerializer(IDbConnection connection, string schemaName)
        {
            throw new System.NotImplementedException();
        }
    }
}
