using System.Data;

// todo clean up
namespace TauCode.Db
{
    public interface IDbUtilityFactory
    {
        //string DbProviderName { get; }
        //IDbConnection CreateConnection();
        IDbDialect GetDialect();
        IDbScriptBuilder CreateScriptBuilder();
        IDbInspector CreateDbInspector(IDbConnection connection);
        IDbTableInspector CreateTableInspector(IDbConnection connection, string tableName);
        IDbCruder CreateCruder(IDbConnection connection);
        IDbSerializer CreateDbSerializer(IDbConnection connection);
        IDbConverter CreateDbConverter();
    }
}
