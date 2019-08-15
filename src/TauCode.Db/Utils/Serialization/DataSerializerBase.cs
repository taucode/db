using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TauCode.Db.Data;
using TauCode.Db.Model;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Crud;
using TauCode.Db.Utils.Inspection;

namespace TauCode.Db.Utils.Serialization
{
    public abstract class DataSerializerBase : IDataSerializer
    {
        #region Nested

        protected class ParameterInfo
        {
            public DbType DbType { get; set; }
            public int? Size { get; set; }

            public int? Precision { get; set; }

            public int? Scale { get; set; }
        }

        #endregion

        #region Fields

        private ICruder _cruder;
        private IScriptBuilder _scriptBuilder;

        #endregion

        #region Constructor

        protected DataSerializerBase()
        {
        }

        #endregion

        #region Polymorph

        protected abstract ICruder CreateCruder();

        protected abstract IScriptBuilder CreateScriptBuilder();

        protected abstract IDbInspector GetDbInspector(IDbConnection connection);

        protected virtual string SerializeCommandResultImpl(IDbCommand command)
        {
            var rows = this.Cruder.GetRows(command);
            var json = JsonConvert.SerializeObject(rows, Formatting.Indented);
            return json;
        }

        protected virtual void DeserializeTableData(IDbConnection connection, TableMold tableMold, JArray tableData)
        {
            var tableName = tableMold.Name;

            if (tableData.Count == 0)
            {
                return; // nothing to deserialize
            }

            // take first entry as standard
            var standard = tableData[0] as JObject;
            if (standard == null)
            {
                throw new ArgumentException("Each row must be represented by a JSON object.", nameof(tableData));
            }

            var standardPropertyNamesSignature = GetJObjectPropertyNamesSignature(standard);

            var standardPropertyNames = standard
                .Properties()
                .Select(x => x.Name)
                .ToArray();

            // each column must be in table
            var columnMapping = new Dictionary<string, ColumnMold>();

            foreach (var standardPropertyName in standardPropertyNames)
            {
                var tableColumn = tableMold.Columns.SingleOrDefault(x =>
                    string.Equals(x.Name, standardPropertyName));

                if (tableColumn == null)
                {
                    throw new ArgumentException(
                        $"JSON contains '{standardPropertyName}' property, but table '{tableName}' does not contain such column.");
                }

                columnMapping.Add(standardPropertyName, tableColumn);
            }

            var sql = this.ScriptBuilder.BuildParameterizedInsertSql(
                tableMold,
                out var parameterMapping,
                columnsToInclude: standardPropertyNames,
                indent: 4);

            var command = connection.CreateCommand();
            command.CommandText = sql;

            var parametersByColumnName = new Dictionary<string, IDbDataParameter>();

            foreach (var pair in parameterMapping)
            {
                var columnName = pair.Key;
                var parameterName = pair.Value;

                var parameter = command.CreateParameter();
                parameter.ParameterName = parameterName;

                ParameterInfo parameterInfo = this.GetParameterInfo(tableMold, columnName);

                if (parameterInfo == null)
                {
                    throw new InvalidOperationException($"'{nameof(GetParameterInfo)}' returned null. Table name: '{tableMold.Name}', column name: '{columnName}'");
                }

                parameter.DbType = parameterInfo.DbType;
                if (parameterInfo.Size.HasValue)
                {
                    parameter.Size = parameterInfo.Size.Value;
                }

                if (parameterInfo.Precision.HasValue)
                {
                    parameter.Precision = (byte)parameterInfo.Precision.Value;
                }

                if (parameterInfo.Scale.HasValue)
                {
                    parameter.Scale = (byte)parameterInfo.Scale.Value;
                }

                command.Parameters.Add(parameter);

                parametersByColumnName.Add(columnName, parameter);
            }

            command.Prepare();

            using (command)
            {
                for (var i = 0; i < tableData.Count; i++)
                {
                    var token = tableData[i];

                    if (token.Type != JTokenType.Object)
                    {
                        throw new ArgumentException("Each row must be represented by a JSON object.", nameof(tableData));
                    }

                    var columnValues = new Dictionary<string, object>();
                    var tokenObject = (JObject)token;

                    var signature = GetJObjectPropertyNamesSignature(tokenObject);
                    if (signature != standardPropertyNamesSignature)
                    {
                        throw new ArgumentException("All rows must have same properties.", nameof(tableData));
                    }

                    foreach (var property in tokenObject.Properties())
                    {
                        var name = property.Name;
                        var jvalue = property.Value;

                        var column = columnMapping[name];

                        var value = this.JsonValueToColumnValue(column, jvalue);
                        columnValues.Add(name, value);

                        var parameter = parametersByColumnName[name];
                        parameter.Value = value ?? DBNull.Value;
                    }

                    command.ExecuteNonQuery();

                    var row = new DynamicRow(columnValues);
                    this.RowDeserialized?.Invoke(tableName, i, row);
                }
            }
        }

        protected virtual object JsonValueToColumnValue(ColumnMold column, JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.String:
                    if (column.Type.Name.ToLower() == "uniqueidentifier")
                    {
                        return new Guid((string)((JValue)token).Value);
                    }
                    else
                    {
                        return (string)((JValue)token).Value;
                    }

                case JTokenType.Float:
                    return (double)((JValue)token).Value;

                case JTokenType.Integer:
                    var longValue = (long)((JValue)token).Value;
                    if (column.Type.Name.ToLower() == "int")
                    {
                        return (int)longValue;
                    }
                    else if (column.Type.Name.ToLower() == "bigint")
                    {
                        return longValue;
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }

                case JTokenType.Boolean:
                    return (bool)((JValue)token).Value;

                case JTokenType.Null:
                    return null;

                case JTokenType.Date:
                    return (DateTime)((JValue)token).Value;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        protected virtual ParameterInfo GetParameterInfo(TableMold tableMold, string columnName)
        {
            ParameterInfo parameterInfo;

            var column = tableMold.GetColumn(columnName);
            var typeName = column.Type.Name.ToLower();

            if (typeName == "uniqueidentifier")
            {
                parameterInfo = new ParameterInfo
                {
                    DbType = DbType.Guid,
                };
            }
            else if (typeName == "varchar")
            {
                parameterInfo = new ParameterInfo
                {
                    DbType = DbType.AnsiString,
                    Size = column.Type.Size,
                };
            }
            else if (typeName == "nvarchar")
            {
                parameterInfo = new ParameterInfo
                {
                    DbType = DbType.String,
                    Size = column.Type.Size,
                };
            }
            else
            {
                parameterInfo = null;
            }

            return parameterInfo;
        }

        #endregion

        #region Protected

        protected ICruder Cruder => _cruder ?? (_cruder = this.CreateCruder());

        protected IScriptBuilder ScriptBuilder => _scriptBuilder ?? (_scriptBuilder = this.CreateScriptBuilder());

        #endregion

        #region Private

        private static string GetJObjectPropertyNamesSignature(JObject obj)
        {
            var sb = new StringBuilder();

            foreach (var property in obj.Properties())
            {
                sb.Append(property.Name);
                sb.Append(";");
            }

            return sb.ToString();
        }

        #endregion

        #region IDataSerializer Members

        public string SerializeCommandResult(IDbCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            var json = this.SerializeCommandResultImpl(command);
            return json;
        }

        public string SerializeTable(IDbConnection connection, string tableName)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var dbInspector = this.GetDbInspector(connection);
            var tableInspector = dbInspector.GetTableInspector(tableName);
            var tableMold = tableInspector.GetTableMold();
            var sql = this.ScriptBuilder.BuildSelectSql(tableMold);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = sql;
                return this.SerializeCommandResultImpl(command);
            }
        }

        public string SerializeDb(IDbConnection connection)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            var dbInspector = this.GetDbInspector(connection);
            var tableMolds = dbInspector.GetOrderedTableMolds(true);

            var dbData = new DynamicRow(); // it is strange to store entire data in 'dynamic' 'row', but why to invent new dynamic ancestor?

            using (var command = connection.CreateCommand())
            {
                foreach (var tableMold in tableMolds)
                {
                    var sql = this.ScriptBuilder.BuildSelectSql(tableMold);
                    command.CommandText = sql;
                    var rows = this.Cruder.GetRows(command);
                    dbData.SetValue(tableMold.Name, rows);
                }
            }

            var json = JsonConvert.SerializeObject(dbData, Formatting.Indented);
            return json;
        }

        public void DeserializeTable(IDbConnection connection, string tableName, string json)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

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

            var dbInspector = this.GetDbInspector(connection);
            var tableInspector = dbInspector.GetTableInspector(tableName);
            var tableMold = tableInspector.GetTableMold();

            this.DeserializeTableData(connection, tableMold, tableData);
        }

        public void DeserializeDb(IDbConnection connection, string json)
        {
            var dbData = JsonConvert.DeserializeObject(json) as JObject;
            if (dbData == null)
            {
                throw new ArgumentException("Could not deserialize DB data.", nameof(json));
            }

            var dbInspector = this.GetDbInspector(connection);

            foreach (var property in dbData.Properties())
            {
                var name = property.Name;
                var tableData = property.Value as JArray;

                if (tableData == null)
                {
                    throw new ArgumentException("Invalid data.", nameof(json));
                }

                var tableInspector = dbInspector.GetTableInspector(name);
                var tableMold = tableInspector.GetTableMold();

                this.DeserializeTableData(connection, tableMold, tableData);
            }
        }

        public event Action<string, int, object> RowDeserialized;

        #endregion
    }
}

