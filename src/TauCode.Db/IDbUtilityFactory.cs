using System.Data;
using TauCode.Db.Schema;

namespace TauCode.Db
{
    public interface IDbUtilityFactory
    {
        IDbDialect GetDialect();
        IDbScriptBuilder CreateScriptBuilder(string schemaName);
        IDbConnection CreateConnection();
        IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection);
        IDbInspector CreateInspector(IDbConnection connection, string schemaName);
        IDbTableInspector CreateTableInspector(IDbConnection connection, string schemaName, string tableName);
        IDbCruder CreateCruder(IDbConnection connection, string schemaName);
        IDbSerializer CreateSerializer(IDbConnection connection, string schemaName);
    }
}
