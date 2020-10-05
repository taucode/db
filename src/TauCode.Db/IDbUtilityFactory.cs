using System.Data;

namespace TauCode.Db
{
    public interface IDbUtilityFactory
    {
        IDbDialect GetDialect();
        IDbScriptBuilder CreateScriptBuilder(string schema);
        IDbConnection CreateConnection();
        IDbInspector CreateInspector(IDbConnection connection, string schema);
        IDbTableInspector CreateTableInspector(IDbConnection connection, string schema, string tableName);
        IDbCruder CreateCruder(IDbConnection connection, string schema);
        IDbSerializer CreateSerializer(IDbConnection connection, string schema);
    }
}
