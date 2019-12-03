using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Data;
using TauCode.Db.Data;
using TauCode.Db.Exceptions;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Inspection;

namespace TauCode.Db.Utils.Crud
{
    public abstract class CruderBase : ICruder
    {
        #region Fields

        private readonly IDbConnection _connection;
        private IScriptBuilder _scriptBuilder;
        private IDbInspector _dbInspector;

        #endregion

        #region Constructor

        protected CruderBase(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        #endregion

        #region Polymorph

        protected abstract string ExpectedDbConnectionTypeFullName { get; }

        protected abstract IScriptBuilder CreateScriptBuilder();

        protected abstract IDbInspector CreateDbInspector();

        #endregion

        #region Protected

        protected IDbConnection GetSafeConnection()
        {
            if (_connection.GetType().FullName == this.ExpectedDbConnectionTypeFullName)
            {
                return _connection;
            }

            throw new TypeMismatchException(
                $"Expected DB connection type is '{this.ExpectedDbConnectionTypeFullName}', but an instance of '{_connection.GetType().FullName}' was provided.");
        }

        #endregion

        #region ICruder Members

        public IDbInspector DbInspector => _dbInspector ?? (_dbInspector = this.CreateDbInspector());

        public IScriptBuilder ScriptBuilder => _scriptBuilder ?? (_scriptBuilder = this.CreateScriptBuilder());

        public void InsertRow(string tableName, object row)
        {
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

            var connection = this.GetSafeConnection();

            var tableInspector = this.DbInspector.GetTableInspector(tableName);
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

        public bool DeleteRow(string tableName, object id)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var tableInspector = this.DbInspector.GetTableInspector(tableName);
            var tableMold = tableInspector.GetTableMold();
            var connection = this.GetSafeConnection();


            using (var command = connection.CreateCommand())
            {
                command.CommandText = this.ScriptBuilder.BuildDeleteRowByIdSql(tableMold, out var paramName);
                command.AddParameterWithValue(paramName, id);

                var deletedCount = command.ExecuteNonQuery();
                return deletedCount == 1;
            }
        }

        public dynamic GetRow(string tableName, object id)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var tableInspector = this.DbInspector.GetTableInspector(tableName);
            var tableMold = tableInspector.GetTableMold();
            var connection = this.GetSafeConnection();

            using (var command = connection.CreateCommand())
            {
                command.CommandText = this.ScriptBuilder.BuildSelectRowByIdSql(tableMold, out var paramName);
                command.AddParameterWithValue(paramName, id);

                var rows = UtilsHelper.GetCommandRows(command);
                return rows.SingleOrDefault();
            }
        }

        public bool UpdateRow(string tableName, object rowUpdate, object id)
        {
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

            var tableInspector = this.DbInspector.GetTableInspector(tableName);
            var tableMold = tableInspector.GetTableMold();
            var connection = this.GetSafeConnection();

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
