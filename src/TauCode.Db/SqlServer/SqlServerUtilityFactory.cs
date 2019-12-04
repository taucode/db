using System;
using System.Data;

namespace TauCode.Db.SqlServer
{
    public class SqlServerUtilityFactory : IUtilityFactory
    {
        public static SqlServerUtilityFactory Instance { get; } = new SqlServerUtilityFactory();

        private SqlServerUtilityFactory()
        {
        }

        public string DbProviderName => DbProviderNames.SqlServer;

        public IDialect GetDialect() => SqlServerDialect.Instance;

        public IScriptBuilder CreateScriptBuilder() => throw new NotImplementedException();

        public IDbInspector CreateDbInspector(IDbConnection connection) => new SqlServerInspector(connection);

        public ITableInspector CreateTableInspector(IDbConnection connection, string tableName) =>
            new SqlServerTableInspector(connection, tableName);

        public ICruder CreateCruder(IDbConnection connection) => new SqlServerCruder(connection);

        public IDbSerializer CreateDbSerializer(IDbConnection connection) => new SqlServerSerializer(connection);
    }
}
