namespace TauCode.Db
{
    public interface ICruder : IUtility
    {
        //IDbInspector DbInspector { get; }
        //IScriptBuilder ScriptBuilder { get; }
        void InsertRow(string tableName, object row);
        bool DeleteRow(string tableName, object id);
        dynamic GetRow(string tableName, object id);
        bool UpdateRow(string tableName, object rowUpdate, object id);
    }
}
