using System.Data;

namespace TauCode.Db
{
    public interface IUtilityFactory
    {
        string DbProviderName { get; }
        IDialect GetDialect();
        IScriptBuilder CreateScriptBuilder();
        IDbInspector CreateDbInspector(IDbConnection connection);
        ITableInspector CreateTableInspector(IDbConnection connection, string tableName);
        ICruder CreateCruder(IDbConnection connection);
        IDbSerializer CreateDbSerializer(IDbConnection connection);
    }
}
