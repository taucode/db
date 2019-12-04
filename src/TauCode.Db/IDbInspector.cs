namespace TauCode.Db
{
    public interface IDbInspector : IUtility
    {
        //IDbConnection Connection { get; }

        //IScriptBuilder CreateScriptBuilder();

        string[] GetTableNames(bool? independentFirst = null);

        ITableInspector GetTableInspector(string tableName);
    }
}
