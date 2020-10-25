using System;
using TauCode.Db.Data;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public interface IDbSerializer : IDbUtility
    {
        string SchemaName { get; }

        IDbCruder Cruder { get; }

        string SerializeTableData(string tableName);

        string SerializeDbData(Func<string, bool> tableNamePredicate = null);

        void DeserializeTableData(
            string tableName,
            string json,
            Func<TableMold, DynamicRow, DynamicRow> rowTransformer = null);

        void DeserializeDbData(
            string json,
            Func<string, bool> tableNamePredicate = null,
            Func<TableMold, DynamicRow, DynamicRow> rowTransformer = null);

        string SerializeTableMetadata(string tableName);

        string SerializeDbMetadata(Func<string, bool> tableNamePredicate = null);
    }
}
