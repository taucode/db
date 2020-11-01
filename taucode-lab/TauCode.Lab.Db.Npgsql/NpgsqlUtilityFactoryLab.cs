using System;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.Npgsql
{
    public class NpgsqlUtilityFactoryLab : IDbUtilityFactory
    {
        public static NpgsqlUtilityFactoryLab Instance { get; } = new NpgsqlUtilityFactoryLab();

        private NpgsqlUtilityFactoryLab()
        {   
        }

        public IDbDialect GetDialect() => NpgsqlDialectLab.Instance;

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
