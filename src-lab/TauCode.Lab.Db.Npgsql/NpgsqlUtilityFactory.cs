using System;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.Npgsql
{
    public class NpgsqlUtilityFactory : IDbUtilityFactory
    {
        public static NpgsqlUtilityFactory Instance { get; } = new NpgsqlUtilityFactory();

        private NpgsqlUtilityFactory()
        {
        }

        public IDbDialect GetDialect() => NpgsqlDialect.Instance;

        public IDbScriptBuilder CreateScriptBuilder(string schema) => new NpgsqlScriptBuilder(schema);

        public IDbInspector CreateDbInspector(IDbConnection connection, string schema) => new NpgsqlInspector(connection, schema);

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string schema, string tableName) =>
            new NpgsqlTableInspector(connection, schema, tableName);

        public IDbCruder CreateCruder(IDbConnection connection, string schema) => new NpgsqlCruder(connection, schema);

        public IDbSerializer CreateDbSerializer(IDbConnection connection, string schema) => new NpgsqlSerializer(connection, schema);

        public IDbConverter CreateDbConverter() => throw new NotSupportedException();
    }
}
