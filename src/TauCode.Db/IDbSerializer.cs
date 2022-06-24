using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public interface IDbSerializer : IDbUtility
    {
        string SchemaName { get; }

        IDbCruder Cruder { get; }

        JsonSerializerSettings JsonSerializerSettings { get; set; }

        string SerializeTableData(string tableName);

        string SerializeDbData(Func<string, bool> tableNamePredicate = null);

        Func<TableMold, IReadOnlyList<object>, IReadOnlyList<object>> BeforeDeserializeTableData { get; set; }

        Action<TableMold, IReadOnlyList<object>> AfterDeserializeTableData { get; set; }

        void DeserializeTableData(
            string tableName,
            string json);

        void DeserializeDbData(
            string json,
            Func<string, bool> tableNamePredicate = null);

        string SerializeTableMetadata(string tableName);

        string SerializeDbMetadata(Func<string, bool> tableNamePredicate = null);
    }
}
