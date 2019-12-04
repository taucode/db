﻿using System;
using System.Data;

namespace TauCode.Db.MySql
{
    public class MySqlUtilityFactory : IUtilityFactory
    {
        public static MySqlUtilityFactory Instance { get; } = new MySqlUtilityFactory();

        private MySqlUtilityFactory()
        {   
        }

        public string DbProviderName => DbProviderNames.MySql;

        public IDialect GetDialect() => MySqlDialect.Instance;

        public IScriptBuilder CreateScriptBuilder() => throw new NotImplementedException();

        public IDbInspector CreateDbInspector(IDbConnection connection) => new MySqlInspector(connection);

        public ICruder CreateCruder(IDbConnection connection) => new MySqlCruder(connection);

        public IDbSerializer CreateDbSerializer(IDbConnection connection) => new MySqlSerializer(connection);
    }
}
