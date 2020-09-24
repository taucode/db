using System;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlUtilityFactory : IDbUtilityFactory
    {
        public static SqlUtilityFactory Instance { get; } = new SqlUtilityFactory();

        private SqlUtilityFactory()
        {
        }

        public IDbDialect GetDialect() => SqlDialect.Instance;

        public IDbScriptBuilder CreateScriptBuilder() => new SqlScriptBuilder();

        public IDbInspector CreateDbInspector(IDbConnection connection) => new SqlInspector(connection);

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string tableName) =>
            new SqlTableInspector(connection, tableName);

        public IDbCruder CreateCruder(IDbConnection connection) => new SqlCruder(connection);

        public IDbSerializer CreateDbSerializer(IDbConnection connection) => new SqlSerializer(connection);

        public IDbConverter CreateDbConverter() => throw new NotSupportedException();
    }
}
