using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Data;
using TauCode.Db.Data;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;

// todo clean up
namespace TauCode.Db
{
    public abstract class DbCruderBase : DbUtilityBase, IDbCruder
    {
        #region Nested

        protected class CommandHelper : IDisposable
        {
            #region Fields

            private readonly TableMold _table;
            private readonly DbCruderBase _cruder;
            private IDbCommand _command;
            private bool _commandPrepared;

            private readonly IReadOnlyDictionary<string, ColumnMold> _columnsByColumnName;
            private readonly IReadOnlyDictionary<string, string> _parameterNamesByColumnNames;
            private readonly IReadOnlyDictionary<string, IDbDataParameter> _parametersByParameterNames;

            /// <summary>
            /// Some DB providers (namely, MySQL) change parameter's size when executing command despite they were never asked for that.
            /// So we gotta keep parameter sizes intact.
            /// </summary>
            private readonly IReadOnlyDictionary<string, int> _parameterSizes;

            #endregion

            #region Constructor

            internal CommandHelper(
                DbCruderBase cruder,
                TableMold table,
                IEnumerable<string> columnNames)
            {
                if (columnNames == null)
                {
                    throw new ArgumentNullException(nameof(columnNames));
                }

                _table = table ?? throw new ArgumentNullException(nameof(table));

                _cruder = cruder ?? throw new ArgumentNullException(nameof(cruder));
                _command = _cruder.Connection.CreateCommand();

                _columnsByColumnName = this.BuildColumnsByColumnName(table, columnNames);
                _parameterNamesByColumnNames = this.BuildParameterNamesByColumnNames();
                _parametersByParameterNames = this.BuildParametersByParameterNames();
                _parameterSizes = _parametersByParameterNames
                    .ToDictionary(x => x.Key, x => x.Value.Size);

                foreach (var parameter in _parametersByParameterNames.Values)
                {
                    _command.Parameters.Add(parameter);
                }
            }

            #endregion

            #region Private

            private IReadOnlyDictionary<string, ColumnMold> BuildColumnsByColumnName(
                TableMold table,
                IEnumerable<string> columnNames)
            {
                return columnNames
                    .Select(x =>
                    {
                        var column = table.Columns.SingleOrDefault(y => y.Name == x);

                        if (column == null)
                        {
                            throw new TauDbException($"Column not found: '{x}'.");
                        }

                        return column;
                    })
                    .ToDictionary(x => x.Name, x => x);
            }

            private IReadOnlyDictionary<string, string> BuildParameterNamesByColumnNames()
            {
                return _columnsByColumnName
                    .ToDictionary(
                        x => x.Key,
                        x => $"p_{x.Key}");
            }

            private IReadOnlyDictionary<string, IDbDataParameter> BuildParametersByParameterNames()
            {
                var result = new Dictionary<string, IDbDataParameter>();

                foreach (var pair in _parameterNamesByColumnNames)
                {
                    var columnName = pair.Key;
                    var parameterName = pair.Value;

                    var column = _columnsByColumnName[columnName];

                    var parameter = _cruder.CreateParameter(_table.Name, column);
                    parameter.ParameterName = parameterName;

                    result.Add(parameterName, parameter);
                }

                return result;
            }

            private void ApplyValuesToCommand(object values)
            {
                if (!_commandPrepared)
                {
                    this.PrepareCommand();
                }

                var rowDictionary = _cruder.ObjectToDataDictionary(values);
                foreach (var pair in rowDictionary)
                {
                    var columnName = pair.Key;
                    var originalColumnValue = pair.Value;

                    if (!_parameterNamesByColumnNames.ContainsKey(columnName))
                    {
                        continue; // row has a value we will not insert into any table's column.
                    }

                    var parameterName = _parameterNamesByColumnNames[columnName];
                    var parameter = _parametersByParameterNames[parameterName];

                    var tableValuesConverter = _cruder.GetTableValuesConverter(_table.Name);
                    var dbValueConverter = tableValuesConverter.GetColumnConverter(columnName);
                    var columnValue = dbValueConverter.ToDbValue(originalColumnValue);

                    if (columnValue == null)
                    {
                        throw new TauDbException(
                            $"Could not transform value '{originalColumnValue}' of type '{originalColumnValue.GetType().FullName}'. Table name is '{_table.Name}'. Column name is '{columnName}'.");
                    }

                    parameter.Size = _parameterSizes[parameter.ParameterName];
                    parameter.Value = columnValue;
                }
            }

            private void PrepareCommand()
            {
                _command.Prepare();
                _commandPrepared = true;
            }

            #endregion

            #region Internal

            internal string CommandText
            {
                get => _command.CommandText;
                set => _command.CommandText = value;
            }

            internal int ExecuteWithValues(object values)
            {
                if (values == null)
                {
                    throw new ArgumentNullException(nameof(values));
                }

                this.ApplyValuesToCommand(values);

                var result = _command.ExecuteNonQuery();
                return result;
            }

            internal IList<dynamic> FetchWithValues(object values)
            {
                this.ApplyValuesToCommand(values);
                var rows = DbTools.GetCommandRows(_command, _cruder.GetTableValuesConverter(_table.Name));

                return rows;
            }

            internal IReadOnlyDictionary<string, string> GetParameterNames() => _parameterNamesByColumnNames;

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                _command?.Dispose();
                _command = null;
            }

            #endregion
        }

        #endregion

        #region Fields

        private IDbScriptBuilder _scriptBuilder;
        private readonly Dictionary<string, IDbTableValuesConverter> _tableValuesConverters;

        #endregion

        #region Constructor

        protected DbCruderBase(IDbConnection connection, string schemaName)
            : base(connection, true, false)
        {
            this.SchemaName = schemaName;
            _tableValuesConverters = new Dictionary<string, IDbTableValuesConverter>();
        }

        #endregion

        #region Abstract

        protected abstract IDbValueConverter CreateDbValueConverter(ColumnMold column);

        protected abstract IDbDataParameter CreateParameter(string tableName, ColumnMold column);

        #endregion

        #region Protected

        protected virtual IDictionary<string, object> ObjectToDataDictionary(object obj)
        {
            IDictionary<string, object> dictionary;

            if (obj is IDictionary<string, object> dictionaryParam)
            {
                dictionary = dictionaryParam;
            }
            else if (obj is DynamicRow dynamicRow)
            {
                dictionary = dynamicRow.ToDictionary();
            }
            else
            {
                dictionary = new ValueDictionary(obj);
            }

            return dictionary;
        }

        protected virtual IDbTableValuesConverter CreateTableValuesConverter(string tableName)
        {
            var tableInspector = this.Factory.CreateTableInspector(this.Connection, this.SchemaName, tableName);
            var table = tableInspector.GetTable();

            var dictionary = table.Columns
                .ToDictionary(
                    x => x.Name,
                    this.CreateDbValueConverter);

            var tableValuesConverter = new DbTableValuesConverter(dictionary);
            return tableValuesConverter;
        }

        #endregion

        #region IDbCruder Members

        public string SchemaName { get; }
        public virtual IDbScriptBuilder ScriptBuilder => _scriptBuilder ??= this.Factory.CreateScriptBuilder(this.SchemaName);

        public IDbTableValuesConverter GetTableValuesConverter(string tableName)
        {
            // todo: checks, lowercase, everywhere.
            var tableValuesConverter = _tableValuesConverters.GetValueOrDefault(tableName);
            if (tableValuesConverter == null)
            {
                tableValuesConverter = this.CreateTableValuesConverter(tableName);
                _tableValuesConverters.Add(tableName, tableValuesConverter);
            }

            return tableValuesConverter;
        }

        public void ResetTableValuesConverters()
        {
            _tableValuesConverters.Clear();
        }

        public virtual void InsertRow(
            string tableName,
            object row,
            Func<string, bool> propertySelector)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            this.InsertRows(tableName, new List<object> { row }, propertySelector);
        }

        public virtual void InsertRows(
            string tableName,
            IReadOnlyList<object> rows,
            Func<string, bool> propertySelector)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (rows == null)
            {
                throw new ArgumentNullException(nameof(rows));
            }

            //if (columnsToOmit != null)
            //{
            //    if (columnsToOmit.Any(string.IsNullOrWhiteSpace))
            //    {
            //        throw new ArgumentException($"'{nameof(columnsToOmit)}' must not contain empty strings or nulls.");
            //    }
            //}

            var table = this.Factory.CreateTableInspector(this.Connection, this.SchemaName, tableName).GetTable();

            if (rows.Count == 0)
            {
                return; // nothing to insert
            }

            //columnsToOmit ??= new List<string>();

            var columnNames = this.ObjectToDataDictionary(rows[0])
                .Keys
                .Where(propertySelector);

            using var helper = new CommandHelper(
                this,
                table,
                columnNames);

            var sql = this.ScriptBuilder.BuildInsertScript(
                table,
                helper.GetParameterNames());

            helper.CommandText = sql;

            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];

                if (row == null)
                {
                    throw new ArgumentException($"'{nameof(rows)}' must not contain nulls.");
                }

                helper.ExecuteWithValues(row);
                this.RowInsertedCallback?.Invoke(tableName, row, i);
            }
        }

        public Action<string, object, int> RowInsertedCallback { get; set; }

        public virtual dynamic GetRow(string tableName, object id)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var table = this.Factory
                .CreateTableInspector(this.Connection, this.SchemaName, tableName)
                .GetTable();

            var idColumnName = table.GetPrimaryKeyColumn().Name;

            using var helper = new CommandHelper(this, table, new[] { idColumnName });
            var sql = this.ScriptBuilder.BuildSelectByIdScript(table, helper.GetParameterNames().Single().Value);
            helper.CommandText = sql;
            var rows = helper.FetchWithValues(new Dictionary<string, object>
            {
                {idColumnName, id}
            });

            return rows.SingleOrDefault();
        }

        public virtual IList<dynamic> GetAllRows(string tableName)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var table = this.Factory
                .CreateTableInspector(this.Connection, this.SchemaName, tableName)
                .GetTable();

            using var command = this.Connection.CreateCommand();
            var sql = this.ScriptBuilder.BuildSelectAllScript(table);
            command.CommandText = sql;
            var rows = DbTools.GetCommandRows(command, this.GetTableValuesConverter(tableName));
            return rows;
        }

        public virtual bool UpdateRow(string tableName, object rowUpdate, object id)
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

            var table = this.Factory
                .CreateTableInspector(this.Connection, this.SchemaName, tableName)
                .GetTable();

            var dataDictionary = this.ObjectToDataDictionary(rowUpdate);

            if (dataDictionary.Keys.Contains(table.GetPrimaryKeyColumn().Name))
            {
                throw new TauDbException("Update object must not contain ID column.");
            }

            var columnNames = new List<string>(dataDictionary.Keys)
            {
                table.GetPrimaryKeyColumn().Name,
            };

            using var helper = new CommandHelper(this, table, columnNames);
            var sql = this.ScriptBuilder.BuildUpdateScript(
                table,
                helper.GetParameterNames());

            dataDictionary.Add(table.GetPrimaryKeyColumn().Name, id);

            helper.CommandText = sql;
            var result = helper.ExecuteWithValues(dataDictionary);
            return result > 0;
        }

        public virtual bool DeleteRow(string tableName, object id)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var table = this.Factory
                .CreateTableInspector(this.Connection, this.SchemaName, tableName)
                .GetTable();

            var idColumnName = table.GetPrimaryKeyColumn().Name;

            using var helper = new CommandHelper(this, table, new[] { idColumnName });
            var sql = this.ScriptBuilder.BuildDeleteByIdScript(table, helper.GetParameterNames().Single().Value);
            helper.CommandText = sql;

            var result = helper.ExecuteWithValues(new Dictionary<string, object>
            {
                {idColumnName, id}
            });

            return result > 0;
        }

        #endregion
    }
}
