using System.Data;

// todo clean up
namespace TauCode.Db
{
    public interface IDbUtilityFactory
    {
        //string DbProviderName { get; }
        //IDbConnection CreateConnection();
        IDbDialect GetDialect();
        IDbScriptBuilder CreateScriptBuilder(string schema);
        IDbInspector CreateDbInspector(IDbConnection connection, string schema);
        IDbTableInspector CreateTableInspector(IDbConnection connection, string schema, string tableName);
        IDbCruder CreateCruder(IDbConnection connection, string schema);
        IDbSerializer CreateDbSerializer(IDbConnection connection, string schema);
        IDbConverter CreateDbConverter();
    }
}
