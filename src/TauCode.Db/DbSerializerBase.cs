using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db.Data;
using TauCode.Db.Model;

namespace TauCode.Db
{
    // todo clean up
    public abstract class DbSerializerBase : UtilityBase, IDbSerializer
    {
        #region Nested

        private class DbMetadata
        {
            public IList<TableMold> Tables { get; set; }
        }

        #endregion

        #region Fields

        private IDbInspector _dbInspector;
        private ICruder _cruder;

        #endregion

        #region Constructor

        protected DbSerializerBase(IDbConnection connection)
            : base(connection, true, false)
        {
        }

        #endregion

        #region Polymorph

        protected virtual void DeserializeTableData(TableMold tableMold, JArray tableData)
        {
            var rows = tableData
                .ToList() // todo: get rid of this.
                .Select(x => tableMold
                    .Columns
                    .Select(y => y.Name)
                    .ToDictionary(
                        z => z,
                        z => ((JValue)x[z]).Value))
                .ToList();
        
            this.Cruder.InsertRows(tableMold.Name, rows);
        }

        #endregion

        #region Protected

        protected virtual ICruder Cruder => _cruder ?? (_cruder = this.Factory.CreateCruder(this.Connection));

        protected virtual IDbInspector DbInspector =>
            _dbInspector ?? (_dbInspector = this.Factory.CreateDbInspector(this.Connection));

        #endregion

        #region IDbSerializer Members

        public IScriptBuilderLab ScriptBuilderLab => this.Cruder.ScriptBuilderLab;

        public virtual string SerializeTableData(string tableName)
        {
            var rows = this.Cruder.GetAllRows(tableName);
            var json = JsonConvert.SerializeObject(rows, Formatting.Indented);
            return json;
        }

        public virtual string SerializeDbData(Func<string, bool> tableNamePredicate = null)
        {
            var tables = this.DbInspector.GetTables(true, tableNamePredicate);


            var dbData = new DynamicRow(); // it is strange to store entire data in 'dynamic' 'row', but why to invent new dynamic ancestor?

            using (var command = this.Connection.CreateCommand())
            {
                foreach (var table in tables)
                {
                    var sql = this.ScriptBuilderLab.BuildSelectAllScript(table);
                    command.CommandText = sql;

                    var rows = DbUtils
                        .GetCommandRows(command);

                    dbData.SetValue(table.Name, rows);
                }
            }

            var json = JsonConvert.SerializeObject(dbData, Formatting.Indented);
            return json;
        }

        public virtual void DeserializeTableData(string tableName, string json)
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

            var table = this.Factory.CreateTableInspector(this.Connection, tableName).GetTable();
            this.DeserializeTableData(table, tableData);
        }

        public virtual void DeserializeDbData(string json)
        {
            var dbData = JsonConvert.DeserializeObject(json) as JObject;
            if (dbData == null)
            {
                throw new ArgumentException("Could not deserialize DB data.", nameof(json));
            }


            foreach (var property in dbData.Properties())
            {
                var name = property.Name;
                var tableData = property.Value as JArray;

                if (tableData == null)
                {
                    throw new ArgumentException("Invalid data.", nameof(json));
                }

                throw new NotImplementedException();
                //var tableMold = tableInspector.GetTableMold();

                //this.DeserializeTableData(connection, tableMold, tableData);
            }
        }

        public virtual string SerializeTableMetadata(string tableName)
        {
            var tableInspector = this.Factory.CreateTableInspector(this.Connection, tableName);
            var tableMold = tableInspector.GetTable();

            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            };

            var json = JsonConvert.SerializeObject(
                tableMold,
                new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented,
                    Converters = new List<JsonConverter>
                    {
                        new StringEnumConverter(new CamelCaseNamingStrategy())
                    }
                });

            return json;
        }

        public virtual string SerializeDbMetadata(Func<string, bool> tableNamePredicate = null)
        {
            tableNamePredicate = tableNamePredicate ?? (x => true);

            var tables = this.DbInspector
                .GetTableNames(true)
                .Select(x => this.Factory.CreateTableInspector(this.Connection, x).GetTable())
                .Where(x => tableNamePredicate(x.Name))
                .ToList();

            var metadata = new DbMetadata
            {
                Tables = tables
                    .Select(x => x.CloneTable(false))
                    .ToList(),
            };

            var contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy(),
            };

            var json = JsonConvert.SerializeObject(
                metadata,
                new JsonSerializerSettings
                {
                    ContractResolver = contractResolver,
                    Formatting = Formatting.Indented,
                    Converters = new List<JsonConverter>
                    {
                        new StringEnumConverter(new CamelCaseNamingStrategy())
                    }
                });

            return json;
        }

        #endregion
    }
}
