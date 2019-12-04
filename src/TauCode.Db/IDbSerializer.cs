using System;

namespace TauCode.Db
{
    public interface IDbSerializer : IUtility
    {
        //ICruder Cruder { get; }
        string SerializeTableData(string tableName);
        string SerializeDbData();
        void DeserializeTableData(string tableName, string json);
        void DeserializeDbData(string json);
        string SerializeTableMetadata(string tableName);
        string SerializeDbMetadata(Func<string, bool> tableNamePredicate = null);

        // todo: sweep out. not needed. use IScriptBuilder.
        //void DeserializeTableMetadata(string tableName, string json);

        // todo: sweep out. not needed. use IScriptBuilder.
        //void DeserializeDbMetadata(string json);

        /// <summary>
        /// Callback event for row deserialization.
        /// Delegate arguments:
        /// * Table name;
        /// * Index of the row having been deserialized;
        /// * Data of the row having been deserialized.
        /// </summary>
        event Action<string, int, object> RowDeserialized;
    }
}
