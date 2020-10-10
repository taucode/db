using System.Data;

namespace TauCode.Db
{
    public interface IDbUtilityFactory
    {
        IDbDialect GetDialect();
        IDbScriptBuilder CreateScriptBuilder(string schemaName);
        IDbConnection CreateConnection();
        IDbInspector CreateInspector(IDbConnection connection, string schemaName);
        IDbTableInspector CreateTableInspector(IDbConnection connection, string schemaName, string tableName);
        IDbCruder CreateCruder(IDbConnection connection, string schemaName);
        IDbSerializer CreateSerializer(IDbConnection connection, string schemaName);
    }
}
