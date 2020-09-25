using System;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.MySql
{
    // todo: check schema arg is null
    public class MySqlUtilityFactory : IDbUtilityFactory
    {
        public static MySqlUtilityFactory Instance { get; } = new MySqlUtilityFactory();

        private MySqlUtilityFactory()
        {
        }

        public IDbDialect GetDialect() => MySqlDialect.Instance;

        public IDbScriptBuilder CreateScriptBuilder(string schema) => new MySqlScriptBuilder(schema);

        public IDbInspector CreateDbInspector(IDbConnection connection, string schema) => new MySqlInspector(connection);

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string schema, string tableName) =>
            new MySqlTableInspector(connection, tableName);

        public IDbCruder CreateCruder(IDbConnection connection, string schema) => new MySqlCruder(connection);

        public IDbSerializer CreateDbSerializer(IDbConnection connection, string schema) => new MySqlSerializer(connection);

        public IDbConverter CreateDbConverter() => throw new NotSupportedException();
    }
}
