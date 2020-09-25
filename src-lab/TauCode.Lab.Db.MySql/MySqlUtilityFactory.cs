using System;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql
{
    public class MySqlUtilityFactory : IDbUtilityFactory
    {
        public static MySqlUtilityFactory Instance { get; } = new MySqlUtilityFactory();

        private MySqlUtilityFactory()
        {
        }

        public IDbDialect GetDialect() => MySqlDialect.Instance;

        public IDbScriptBuilder CreateScriptBuilder(string schema) => new MySqlScriptBuilder(schema);

        public IDbInspector CreateDbInspector(IDbConnection connection, string schema) => new MySqlInspector(connection, schema);

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string schema, string tableName) =>
            new MySqlTableInspector(connection, schema, tableName);

        public IDbCruder CreateCruder(IDbConnection connection, string schema) => new MySqlCruder(connection, schema);

        public IDbSerializer CreateDbSerializer(IDbConnection connection, string schema) => new MySqlSerializer(connection, schema);

        public IDbConverter CreateDbConverter() => throw new NotSupportedException();
    }
}
