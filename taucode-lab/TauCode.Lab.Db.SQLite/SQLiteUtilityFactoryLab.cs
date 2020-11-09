﻿using System.Data;
using System.Data.SQLite;
using TauCode.Db;

namespace TauCode.Lab.Db.SQLite
{
    public class SQLiteUtilityFactoryLab : IDbUtilityFactory
    {
        public static SQLiteUtilityFactoryLab Instance { get; } = new SQLiteUtilityFactoryLab();

        private SQLiteUtilityFactoryLab()
        {
        }

        public IDbDialect GetDialect() => SQLiteDialectLab.Instance;

        public IDbScriptBuilder CreateScriptBuilder(string schemaName)
        {
            return new SQLiteScriptBuilderLab();
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
            // todo: check schema is null, here & anywhere.
            return new SQLiteTableInspectorLab((SQLiteConnection)connection, tableName);
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
