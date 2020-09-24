﻿using System;
using System.Data;
using TauCode.Db;

// todo clean up
namespace TauCode.Lab.Db.SqlClient
{
    public class SqlServerUtilityFactory : IUtilityFactory
    {
        public static SqlServerUtilityFactory Instance { get; } = new SqlServerUtilityFactory();

        private SqlServerUtilityFactory()
        {
        }

        //public string DbProviderName => DbProviderNames.SqlServer;

        //public IDbConnection CreateConnection() => DbUtils.CreateConnection(this.DbProviderName);

        public IDialect GetDialect() => SqlServerDialect.Instance;

        public IScriptBuilder CreateScriptBuilder() => new SqlServerScriptBuilder();

        public IDbInspector CreateDbInspector(IDbConnection connection) => new SqlServerInspector(connection);

        public ITableInspector CreateTableInspector(IDbConnection connection, string tableName) =>
            new SqlServerTableInspector(connection, tableName);

        public ICruder CreateCruder(IDbConnection connection) => new SqlServerCruder(connection);

        public IDbSerializer CreateDbSerializer(IDbConnection connection) => new SqlServerSerializer(connection);

        public IDbConverter CreateDbConverter() => throw new NotSupportedException();
    }
}
