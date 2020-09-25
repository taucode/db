﻿using System;
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

        public IDbScriptBuilder CreateScriptBuilder(string schema) => new SqlScriptBuilder(schema);

        public IDbInspector CreateDbInspector(IDbConnection connection, string schema) => new SqlInspector(connection, schema);

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string schema, string tableName) =>
            new SqlTableInspector(connection, schema, tableName);

        public IDbCruder CreateCruder(IDbConnection connection, string schema) => new SqlCruder(connection, schema);

        public IDbSerializer CreateDbSerializer(IDbConnection connection, string schema) => new SqlSerializer(connection, schema);

        public IDbConverter CreateDbConverter() => throw new NotSupportedException();
    }
}