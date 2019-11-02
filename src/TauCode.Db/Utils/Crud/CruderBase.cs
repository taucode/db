using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Data;
using TauCode.Db.Data;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Inspection;

namespace TauCode.Db.Utils.Crud
{
    public abstract class CruderBase : ICruder
    {
        #region Constructor

        protected CruderBase(IScriptBuilder scriptBuilder)
        {
            this.ScriptBuilder = scriptBuilder ?? throw new ArgumentNullException(nameof(scriptBuilder));
        }

        #endregion

        #region Polymorph

        protected abstract ITableInspector GetTableInspectorImpl(IDbConnection connection, string tableName);

        #endregion

        #region ICruder Members

        public IScriptBuilder ScriptBuilder { get; }

        public ITableInspector GetTableInspector(IDbConnection connection, string tableName)
        {
            return this.GetTableInspectorImpl(connection, tableName);
        }

        public void InsertRow(IDbConnection connection, string tableName, object row)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            IDictionary<string, object> dictionary;

            if (row is IDictionary<string, object> dictionaryParam)
            {
                dictionary = dictionaryParam;
            }
            else if (row is DynamicRow dynamicRow)
            {
                dictionary = dynamicRow.ToDictionary();
            }
            else
            {
                dictionary = new ValueDictionary(row);
            }

            var tableInspector = this.GetTableInspector(connection, tableName);
            var tableMold = tableInspector.GetTableMold();

            var script = this.ScriptBuilder.BuildParameterizedInsertSql(
                tableMold,
                out var parameterMapping,
                columnsToInclude: dictionary.Keys.ToArray(),
                indent: 4);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = script;

                foreach (var columnName in parameterMapping.Keys)
                {
                    var parameterName = parameterMapping[columnName];
                    var parameterValue = dictionary[columnName] ?? DBNull.Value;

                    command.AddParameterWithValue(parameterName, parameterValue);
                }

                command.ExecuteNonQuery();
            }
        }

        public bool DeleteRow(IDbConnection connection, string tableName, object id)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var tableMold = this.GetTableInspector(connection, tableName).GetTableMold();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = this.ScriptBuilder.BuildDeleteRowByIdSql(tableMold, out var paramName);
                command.AddParameterWithValue(paramName, id);

                var deletedCount = command.ExecuteNonQuery();
                return deletedCount == 1;
            }
        }

        public List<dynamic> GetRows(IDbCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            using (var reader = command.ExecuteReader())
            {
                var rows = new List<dynamic>();

                while (reader.Read())
                {
                    var row = new DynamicRow(true);

                    for (var i = 0; i < reader.FieldCount; i++)
                    {
                        var name = reader.GetName(i);
                        var value = reader[i];
                        row.SetValue(name, value);
                    }

                    rows.Add(row);
                }

                return rows;
            }
        }

        public dynamic GetRow(IDbConnection connection, string tableName, object id)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var tableMold = this.GetTableInspector(connection, tableName).GetTableMold();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = this.ScriptBuilder.BuildSelectRowByIdSql(tableMold, out var paramName);
                command.AddParameterWithValue(paramName, id);

                var rows = this.GetRows(command);
                return rows.SingleOrDefault();
            }
        }

        public bool UpdateRow(IDbConnection connection, string tableName, object rowUpdate, object id)
        {
            if (connection == null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (rowUpdate == null)
            {
                throw new ArgumentNullException(nameof(rowUpdate));
            }

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            IDictionary<string, object> dictionary;

            if (rowUpdate is IDictionary<string, object> dictionaryParam)
            {
                dictionary = dictionaryParam;
            }
            else if (rowUpdate is DynamicRow dynamicRow)
            {
                dictionary = dynamicRow.ToDictionary();
            }
            else
            {
                dictionary = new ValueDictionary(rowUpdate);
            }

            var tableInspector = this.GetTableInspector(connection, tableName);
            var tableMold = tableInspector.GetTableMold();

            var pkColumnName = tableMold.GetSinglePrimaryKeyColumnName();

            var script = this.ScriptBuilder.BuildUpdateRowByIdSql(
                tableMold.Name,
                pkColumnName,
                dictionary.Keys.ToArray(),
                out var parameterMapping);

            using (var command = connection.CreateCommand())
            {
                command.CommandText = script;

                foreach (var columnName in parameterMapping.Keys)
                {
                    var paramName = parameterMapping[columnName];
                    object paramValue;

                    if (columnName == pkColumnName)
                    {
                        paramValue = id;
                    }
                    else
                    {
                        paramValue = dictionary[columnName] ?? DBNull.Value;
                    }

                    command.AddParameterWithValue(paramName, paramValue);
                }

                var rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0; // actually, should always be 0 or 1.
            }
        }

        #endregion
    }
}

