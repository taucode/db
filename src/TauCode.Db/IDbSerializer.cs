using System;

namespace TauCode.Db
{
    public interface IDbSerializer : IUtility
    {
        IScriptBuilderLab ScriptBuilderLab { get; }
        string SerializeTableData(string tableName);
        string SerializeDbData(Func<string, bool> tableNamePredicate = null);
        void DeserializeTableData(string tableName, string json);
        void DeserializeDbData(string json);
        string SerializeTableMetadata(string tableName);
        string SerializeDbMetadata(Func<string, bool> tableNamePredicate = null);
    }
}
