using System.Data;

namespace TauCode.Db
{
    public interface IDbUtilityFactory
    {
        IDbDialect GetDialect();
        IDbScriptBuilder CreateScriptBuilder(string schema);
        IDbInspector CreateDbInspector(IDbConnection connection, string schema);
        IDbTableInspector CreateTableInspector(IDbConnection connection, string schema, string tableName);
        IDbCruder CreateCruder(IDbConnection connection, string schema);
        IDbSerializer CreateDbSerializer(IDbConnection connection, string schema);
        IDbConverter CreateDbConverter();
    }
}
