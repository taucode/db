using System;
using System.Data;

namespace TauCode.Db.Utils.Serialization
{
    public interface IDataSerializer
    {
        string SerializeCommandResult(IDbCommand command);
        string SerializeTable(IDbConnection connection, string tableName);
        string SerializeDb(IDbConnection connection);

        void DeserializeTable(IDbConnection connection, string tableName, string json);
        void DeserializeDb(IDbConnection connection, string json);

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
