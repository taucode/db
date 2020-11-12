using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db.Data;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public abstract class DbSerializerBase : DbUtilityBase, IDbSerializer
    {
        #region Fields

        private IDbInspector _dbInspector;
        private IDbCruder _cruder;

        #endregion

        #region Constructor

        protected DbSerializerBase(IDbConnection connection, string schemaName)
            : base(connection, true, false)
        {
            this.SchemaName = schemaName;
        }

        #endregion

        #region Polymorph

        protected virtual void DeserializeTableData(
            TableMold tableMold,
            JArray tableData,
            Func<TableMold, DynamicRow, DynamicRow> rowTransformer)
        {
            var rows = new List<object>();

            rowTransformer ??= (tableMoldArg, rowArg) => rowArg;

            var index = 0;

            foreach (var jToken in tableData)
            {
                var row = new DynamicRow();

                foreach (var columnMold in tableMold.Columns)
                {
                    object columnValue = null;

                    var columnName = columnMold.Name;
                    var jProp = jToken[columnName];

                    if (jProp == null)
                    {
                        // remains null
                    }
                    else if (jProp is JValue jValue)
                    {
                        columnValue = jValue.Value;
                    }
                    else
                    {
                        throw new TauDbException($"Object representing row #{index} is invalid. Property '{columnName}' is not a JValue.");
                    }

                    row.SetValue(columnName, columnValue);
                }

                var finalRow = rowTransformer(tableMold, row);
                rows.Add(finalRow);

                index++;
            }

            this.Cruder.InsertRows(tableMold.Name, rows, propName => true);
        }

        #endregion

        #region Protected

        protected virtual IDbInspector DbInspector =>
            _dbInspector ??= this.Factory.CreateInspector(this.Connection, this.SchemaName);

        #endregion

        #region IDbSerializer Members

        public string SchemaName { get; }

        public virtual IDbCruder Cruder => _cruder ??= this.Factory.CreateCruder(this.Connection, this.SchemaName);

        public JsonSerializerSettings JsonSerializerSettings { get; set; } = new JsonSerializerSettings();

        public virtual string SerializeTableData(string tableName)
        {
            var rows = this.Cruder.GetAllRows(tableName);
            var json = JsonConvert.SerializeObject(rows, this.JsonSerializerSettings);
            return json;
        }

        public virtual string SerializeDbData(Func<string, bool> tableNamePredicate = null)
        {
            var tables = this.DbInspector.GetTables(true, tableNamePredicate);
            
            var dbData =
                new DynamicRow(); // it is strange to store entire data in 'dynamic' 'row', but why to invent new dynamic ancestor?

            using (var command = this.Connection.CreateCommand())
            {
                foreach (var table in tables)
                {
                    var sql = this.Cruder.ScriptBuilder.BuildSelectAllScript(table);
                    command.CommandText = sql;

                    var tableValuesConverter = this.Cruder.GetTableValuesConverter(table.Name);

                    var rows = command.GetCommandRows(tableValuesConverter);

                    dbData.SetValue(table.Name, rows);
                }
            }

            var json = JsonConvert.SerializeObject(dbData, this.JsonSerializerSettings);
            return json;
        }

        public virtual void DeserializeTableData(
            string tableName,
            string json,
            Func<TableMold, DynamicRow, DynamicRow> rowTransformer = null)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var tableData = JsonConvert.DeserializeObject(json) as JArray;

            if (tableData == null)
            {
                throw new ArgumentException("Could not deserialize table data as array.", nameof(json));
            }

            var table = this.Factory.CreateTableInspector(this.Connection, this.SchemaName, tableName).GetTable();
            this.DeserializeTableData(table, tableData, rowTransformer);
        }

        public virtual void DeserializeDbData(
            string json,
            Func<string, bool> tableNamePredicate = null,
            Func<TableMold, DynamicRow, DynamicRow> rowTransformer = null)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            var dbData = JsonConvert.DeserializeObject(json) as JObject;
            if (dbData == null)
            {
                throw new ArgumentException("Could not deserialize DB data.", nameof(json));
            }

            foreach (var property in dbData.Properties())
            {
                var name = property.Name;

                var need = tableNamePredicate?.Invoke(name) ?? true;
                if (!need)
                {
                    continue;
                }

                var tableData = property.Value as JArray;

                if (tableData == null)
                {
                    throw new ArgumentException("Invalid data.", nameof(json));
                }

                var tableInspector = this.Factory.CreateTableInspector(this.Connection, this.SchemaName, name);
                var tableMold = tableInspector.GetTable();

                this.DeserializeTableData(tableMold, tableData, rowTransformer);
            }
        }

        public virtual string SerializeTableMetadata(string tableName)
        {
            var tableInspector = this.Factory.CreateTableInspector(this.Connection, this.SchemaName, tableName);
            var table = tableInspector.GetTable().CloneTable(true);

            table.ForeignKeys = table.ForeignKeys
                .OrderBy(x => x.Name)
                .ToList();

            table.Indexes = table.Indexes
                .OrderBy(x => x.Name)
                .ToList();

            var json = JsonConvert.SerializeObject(table, this.JsonSerializerSettings);
            return json;
        }

        public virtual string SerializeDbMetadata(Func<string, bool> tableNamePredicate = null)
        {
            tableNamePredicate ??= (x => true);

            var tables = this.DbInspector
                .GetTables(true, tableNamePredicate);

            foreach (var table in tables)
            {
                table.ForeignKeys = table.ForeignKeys
                    .OrderBy(x => x.Name)
                    .ToList();

                table.Indexes = table.Indexes
                    .OrderBy(x => x.Name)
                    .ToList();
            }

            var dbMold = new DbMold
            {
                Tables = tables
                    .Select(x => x.CloneTable(true))
                    .ToList(),
            };

            var json = JsonConvert.SerializeObject(dbMold, this.JsonSerializerSettings);
            return json;
        }

        #endregion
    }
}
