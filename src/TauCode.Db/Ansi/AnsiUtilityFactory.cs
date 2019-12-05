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

        public IDialect GetDialect() => AnsiDialect.Instance;

        public IScriptBuilderLab CreateScriptBuilderLab() => throw new NotSupportedException();

        public IDbInspector CreateDbInspector(IDbConnection connection) => throw new NotSupportedException();

        public ITableInspector CreateTableInspector(IDbConnection connection, string tableName) => throw new NotSupportedException();

        public ICruder CreateCruder(IDbConnection connection) => throw new NotSupportedException();

        public IDbSerializer CreateDbSerializer(IDbConnection connection) => throw new NotSupportedException();
    }
}
