using System;
using System.Data;

namespace TauCode.Db.Ansi
{
    public class AnsiUtilityFactory : IUtilityFactory
    {
        public static AnsiUtilityFactory Instance { get; } = new AnsiUtilityFactory();

        private AnsiUtilityFactory()
        {   
        }

        public string DbProviderName => DbProviderNames.Ansi;

        public IDbConnection CreateConnection() => throw new NotSupportedException();

        public IDialect GetDialect() => AnsiDialect.Instance;

        public IScriptBuilder CreateScriptBuilder() => throw new NotSupportedException();

        public IDbInspector CreateDbInspector(IDbConnection connection) => throw new NotSupportedException();

        public ITableInspector CreateTableInspector(IDbConnection connection, string tableName) => throw new NotSupportedException();

        public ICruder CreateCruder(IDbConnection connection) => throw new NotSupportedException();

        public IDbSerializer CreateDbSerializer(IDbConnection connection) => throw new NotSupportedException();
        public IDbConverter CreateDbConverter() => throw new NotSupportedException();
    }
}
