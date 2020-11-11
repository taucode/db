using MySql.Data.MySqlClient;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlUtilityFactoryLab : IDbUtilityFactory
    {
        public static MySqlUtilityFactoryLab Instance { get; } = new MySqlUtilityFactoryLab();

        private MySqlUtilityFactoryLab()
        {
        }

        public IDbDialect GetDialect() => MySqlDialectLab.Instance;

        public IDbScriptBuilder CreateScriptBuilder(string schemaName) => new MySqlScriptBuilderLab(schemaName);

        public IDbConnection CreateConnection() => new MySqlConnection();

        public IDbInspector CreateInspector(IDbConnection connection, string schemaName)
        {
            return new MySqlInspectorLab((MySqlConnection)connection);
        }

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string schemaName, string tableName)
        {
            return new MySqlTableInspectorLab((MySqlConnection)connection, tableName);
        }

        public IDbCruder CreateCruder(IDbConnection connection, string schemaName)
        {
            return new MySqlCruderLab((MySqlConnection)connection);
        }

        public IDbSerializer CreateSerializer(IDbConnection connection, string schemaName)
        {
            return new MySqlSerializerLab((MySqlConnection)connection);
        }
    }
}
