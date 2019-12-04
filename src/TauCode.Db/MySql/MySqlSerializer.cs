using System;
using System.Data;

namespace TauCode.Db.MySql
{
    public class MySqlSerializer : IDbSerializer
    {
        public MySqlSerializer(IDbConnection connection)
        {
            throw new NotImplementedException();
        }

        public IUtilityFactory Factory => throw new NotImplementedException();
        public string SerializeTableData(string tableName)
        {
            throw new NotImplementedException();
        }

        public string SerializeDbData()
        {
            throw new NotImplementedException();
        }

        public void DeserializeTableData(string tableName, string json)
        {
            throw new NotImplementedException();
        }

        public void DeserializeDbData(string json)
        {
            throw new NotImplementedException();
        }

        public string SerializeTableMetadata(string tableName)
        {
            throw new NotImplementedException();
        }

        public string SerializeDbMetadata(Func<string, bool> tableNamePredicate = null)
        {
            throw new NotImplementedException();
        }

        public event Action<string, int, object> RowDeserialized;
    }
}
