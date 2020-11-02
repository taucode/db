﻿using System;
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

        public IDbDialect GetDialect()
        {
            throw new NotImplementedException();
        }

        public IDbScriptBuilder CreateScriptBuilder(string schemaName)
        {
            throw new NotImplementedException();
        }

        public IDbConnection CreateConnection()
        {
            throw new NotImplementedException();
        }

        public IDbInspector CreateInspector(IDbConnection connection, string schemaName)
        {
            throw new NotImplementedException();
        }

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string schemaName, string tableName)
        {
            throw new NotImplementedException();
        }

        public IDbCruder CreateCruder(IDbConnection connection, string schemaName)
        {
            throw new NotImplementedException();
        }

        public IDbSerializer CreateSerializer(IDbConnection connection, string schemaName)
        {
            throw new NotImplementedException();
        }
    }
}
