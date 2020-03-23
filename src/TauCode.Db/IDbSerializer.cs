using System;

namespace TauCode.Db
{
    public interface IDbSerializer : IUtility
    {
        ICruder Cruder { get; }
        string SerializeTableData(string tableName);
        string SerializeDbData(Func<string, bool> tableNamePredicate = null);
        void DeserializeTableData(string tableName, string json);
        void DeserializeDbData(string json, Func<string, bool> tableNamePredicate = null);
        string SerializeTableMetadata(string tableName);
        string SerializeDbMetadata(Func<string, bool> tableNamePredicate = null);
    }
}
