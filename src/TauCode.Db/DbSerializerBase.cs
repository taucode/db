using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
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

        protected DbSerializerBase(IDbConnection connection)
            : base(connection, true, false)
        {
        }

        #endregion

        #region Polymorph

        protected virtual void DeserializeTableData(TableMold tableMold, JArray tableData)
        {
            var rows = tableData
                .Select((x, xIndex) => tableMold
                    .Columns
                    .Select(y => y.Name)
                    .ToDictionary(
                        z => z,
                        z =>
                        {
                            var jToken = x[z];

                            if (jToken == null)
                            {
                                throw new DbException($"Property '{z}' not found in JSON. Table '{tableMold.Name}', entry index '{xIndex}'.");
                            }

                            if (jToken is JValue jValue)
                            {
                                return jValue.Value;
                            }
                            else
                            {
                                throw new DbException($"Property '{z}' is not a JValue. Table '{tableMold.Name}', entry index '{xIndex}'.");
                            }
                        }))
                .ToList();

            this.Cruder.InsertRows(tableMold.Name, rows);
        }

        #endregion

        #region Protected

        protected virtual IDbInspector DbInspector => _dbInspector ??= this.Factory.CreateDbInspector(this.Connection);

        #endregion

        #region IDbSerializer Members

        public virtual IDbCruder Cruder => _cruder ??= this.Factory.CreateCruder(this.Connection);

        public virtual string SerializeTableData(string tableName)
        {
            var rows = this.Cruder.GetAllRows(tableName);
            var json = JsonConvert.SerializeObject(rows, Formatting.Indented);
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

                    var rows = DbTools
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

        public virtual void DeserializeDbData(string json, Func<string, bool> tableNamePredicate = null)
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

                var tableInspector = this.Factory.CreateTableInspector(this.Connection, name);
                var tableMold = tableInspector.GetTable();

                this.DeserializeTableData(tableMold, tableData);
            }
        }

        public virtual string SerializeTableMetadata(string tableName)
        {
            var tableInspector = this.Factory.CreateTableInspector(this.Connection, tableName);
            var table = tableInspector.GetTable().CloneTable(false);

            table.ForeignKeys = table.ForeignKeys
                .OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase)
                .ToList();

            table.Indexes = table.Indexes
                .OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase)
                .ToList();

            var json = DbTools.FineSerializeToJson(table);
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

            foreach (var table in tables)
            {
                table.ForeignKeys = table.ForeignKeys
                    .OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase)
                    .ToList();

                table.Indexes = table.Indexes
                    .OrderBy(x => x.Name, StringComparer.InvariantCultureIgnoreCase)
                    .ToList();
            }

            var dbMold = new DbMold
            {
                DbProviderName = this.Factory.GetDialect().Name,
                Tables = tables
                    .Select(x => x.CloneTable(false))
                    .ToList(),
            };

            var json = DbTools.FineSerializeToJson(dbMold);
            return json;
        }

        #endregion
    }
}
