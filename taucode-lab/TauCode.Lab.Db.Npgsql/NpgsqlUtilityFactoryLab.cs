using Npgsql;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.Npgsql
{
    public class NpgsqlUtilityFactoryLab : IDbUtilityFactory
    {
        public static NpgsqlUtilityFactoryLab Instance { get; } = new NpgsqlUtilityFactoryLab();

        private NpgsqlUtilityFactoryLab()
        {
        }

        public IDbDialect GetDialect() => NpgsqlDialectLab.Instance;

        public IDbScriptBuilder CreateScriptBuilder(string schemaName) => new NpgsqlScriptBuilderLab(schemaName);

        public IDbConnection CreateConnection() => new NpgsqlConnection();

        public IDbInspector CreateInspector(IDbConnection connection, string schemaName) =>
            new NpgsqlInspectorLab((NpgsqlConnection)connection, schemaName);

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string schemaName, string tableName) =>
            new NpgsqlTableInspectorLab((NpgsqlConnection)connection, schemaName, tableName);

        public IDbCruder CreateCruder(IDbConnection connection, string schemaName) =>
            new NpgsqlCruderLab((NpgsqlConnection)connection, schemaName);

        public IDbSerializer CreateSerializer(IDbConnection connection, string schemaName) =>
            new NpgsqlSerializerLab((NpgsqlConnection)connection, schemaName);
    }
}
