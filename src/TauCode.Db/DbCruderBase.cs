using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Data;
using TauCode.Db.Data;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;

// todo clean up; get rid of TableInspector creation (except of really necessary ones)
namespace TauCode.Db
{
    public abstract class DbCruderBase : DbUtilityBase, IDbCruder
    {
        #region Nested

        protected class TableInfo
        {
            public TableInfo(DbCruderBase cruder, string tableName)
            {
                cruder.CheckSchemaIfNeeded();

                this.TableMold = cruder
                    .Factory
                    .CreateTableInspector(cruder.Connection, cruder.SchemaName, tableName)
                    .GetTable();

                this.PrimaryKeyColumnName = this.TableMold.GetPrimaryKeySingleColumn(nameof(tableName)).Name;
                this.TableValuesConverter = cruder.CreateTableValuesConverter(this.TableMold);
            }

            public TableMold TableMold { get; }
            public string PrimaryKeyColumnName { get; }
            public IDbTableValuesConverter TableValuesConverter { get; }
        }

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
            /// Some DB providers (namely, MySQL) change parameter's size when executing command, even though they were never asked to do so.
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
                            throw new TauDbException($"Column '{x}' does not exist.");
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

                foreach (var columnName in _parameterNamesByColumnNames.Keys)
                {
                    var gotColumn = rowDictionary.TryGetValue(columnName, out var originalColumnValue);
                    if (!gotColumn)
                    {
                        throw new ArgumentException(
                            $"'{nameof(values)}' does not contain property representing column '{columnName}'.",
                            nameof(values));
                    }

                    var parameterName = _parameterNamesByColumnNames[columnName];
                    var parameter = _parametersByParameterNames[parameterName];

                    var tableValuesConverter = _cruder.GetTableValuesConverter(_table.Name);

                    try
                    {
                        var dbValueConverter = tableValuesConverter.GetColumnConverter(columnName);

                        if (dbValueConverter == null)
                        {
                            throw new TauDbException(
                                $"'GetColumnConverter' returned null. Table name is '{_table.Name}'. Column name is '{columnName}'.");
                        }

                        var columnValue = dbValueConverter.ToDbValue(originalColumnValue);

                        if (columnValue == null)
                        {
                            throw new TauDbException(
                                $"Method '{nameof(IDbValueConverter.ToDbValue)}' of the instance of type '{dbValueConverter.GetType().FullName}' returned null. Table: '{this._table.Name}', column: '{columnName}'.");
                        }

                        parameter.Size = _parameterSizes[parameter.ParameterName];
                        parameter.Value = columnValue;
                    }
                    catch (Exception ex)
                    {
                        throw new TauDbException(
                            $"Could not transform value '{originalColumnValue}' of type '{originalColumnValue.GetType().FullName}'. Table name is '{_table.Name}'. Column name is '{columnName}'.",
                            ex);
                    }
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
                var rows = _command.GetCommandRows(_cruder.GetTableValuesConverter(_table.Name));

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
        private readonly Dictionary<string, TableInfo> _tableInfos;

        #endregion

        #region Constructor

        protected DbCruderBase(IDbConnection connection, string schemaName)
            : base(connection, true, false)
        {
            this.SchemaName = schemaName;
            _tableInfos = new Dictionary<string, TableInfo>();
        }

        #endregion

        #region Private

        private static bool PropertyTruer(string propertyName) => true;

        private void CheckSchemaIfNeeded()
        {
            if (this.NeedCheckSchemaExistence)
            {
                if (!this.SchemaExists(this.SchemaName))
                {
                    throw DbTools.CreateSchemaDoesNotExistException(this.SchemaName);
                }
            }
        }

        #endregion

        #region Abstract

        protected abstract IDbValueConverter CreateDbValueConverter(ColumnMold column);

        protected abstract IDbDataParameter CreateParameter(string tableName, ColumnMold column);

        protected abstract bool NeedCheckSchemaExistence { get; }

        protected abstract bool SchemaExists(string schemaName);

        #endregion

        #region Protected

        protected TableInfo GetOrCreateTableInfo(string tableName)
        {
            tableName = this.TransformTableName(tableName);

            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var tableInfo = _tableInfos.GetValueOrDefault(tableName);
            if (tableInfo == null)
            {
                tableInfo = new TableInfo(this, tableName);
                _tableInfos.Add(tableName, tableInfo);
            }

            return tableInfo;
        }

        protected virtual string TransformTableName(string tableName) => tableName;

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

        protected virtual IDbTableValuesConverter CreateTableValuesConverter(TableMold tableMold)
        {
            //var table-Inspector = this.Factory.CreateTable-Inspector(this.Connection, this.SchemaName, tableName);
            //var table = table-Inspector.GetTable();

            var dictionary = tableMold.Columns
                .ToDictionary(
                    x => x.Name,
                    this.CreateDbValueConverter);

            var tableValuesConverter = new DbTableValuesConverter(dictionary);
            return tableValuesConverter;
        }

        #endregion

        #region IDbCruder Members

        public string SchemaName { get; }

        public virtual IDbScriptBuilder ScriptBuilder =>
            _scriptBuilder ??= this.Factory.CreateScriptBuilder(this.SchemaName);

        public IDbTableValuesConverter GetTableValuesConverter(string tableName)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }
        
            return this.GetOrCreateTableInfo(tableName).TableValuesConverter;
        }

        public void ResetTables()
        {
            _tableInfos.Clear();

            //_tableValuesConverters.Clear();
        }

        public virtual void InsertRow(
            string tableName,
            object row,
            Func<string, bool> propertySelector = null)
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
            Func<string, bool> propertySelector = null)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (rows == null)
            {
                throw new ArgumentNullException(nameof(rows));
            }

            propertySelector ??= PropertyTruer;

            //var table = this.Factory.CreateTable-Inspector(this.Connection, this.SchemaName, tableName).GetTable();

            var table = this.GetOrCreateTableInfo(tableName).TableMold;

            if (rows.Count == 0)
            {
                return; // nothing to insert
            }

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
                    throw new ArgumentException($"'{nameof(rows)}' must not contain nulls.", nameof(rows));
                }

                helper.ExecuteWithValues(row);
                this.RowInsertedCallback?.Invoke(tableName, row, i);
            }
        }

        public Action<string, object, int> RowInsertedCallback { get; set; }

        public virtual dynamic GetRow(string tableName, object id, Func<string, bool> columnSelector = null)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var table = this.GetOrCreateTableInfo(tableName).TableMold;

            //var table = this.Factory
            //    .CreateTable-Inspector(this.Connection, this.SchemaName, tableName)
            //    .GetTable();

            var idColumnName = table.GetPrimaryKeySingleColumn(nameof(tableName)).Name;

            using var helper = new CommandHelper(this, table, new[] { idColumnName });
            var sql = this.ScriptBuilder.BuildSelectByPrimaryKeyScript(table, helper.GetParameterNames().Single().Value,
                columnSelector);
            helper.CommandText = sql;
            var rows = helper.FetchWithValues(new Dictionary<string, object>
            {
                {idColumnName, id}
            });

            return rows.SingleOrDefault();
        }

        public virtual IList<dynamic> GetAllRows(string tableName, Func<string, bool> columnSelector = null)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var table = this.GetOrCreateTableInfo(tableName).TableMold;

            using var command = this.Connection.CreateCommand();
            var sql = this.ScriptBuilder.BuildSelectAllScript(table, columnSelector);
            command.CommandText = sql;
            var rows = command.GetCommandRows(this.GetTableValuesConverter(tableName));
            return rows;
        }

        public virtual bool UpdateRow(string tableName, object rowUpdate, Func<string, bool> propertySelector = null)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (rowUpdate == null)
            {
                throw new ArgumentNullException(nameof(rowUpdate));
            }

            propertySelector ??= PropertyTruer;

            var table = this.GetOrCreateTableInfo(tableName).TableMold;

            //var table = this.Factory
            //    .CreateTable-Inspector(this.Connection, this.SchemaName, tableName)
            //    .GetTable();

            var dataDictionary = this.ObjectToDataDictionary(rowUpdate);

            var columnNames = dataDictionary
                .Keys
                .Where(propertySelector)
                .ToHashSet();

            var idColumnName = table.GetPrimaryKeySingleColumn(nameof(tableName)).Name;
            if (!columnNames.Contains(idColumnName))
            {
                throw new ArgumentException("Row update object does not contain primary key value.", nameof(rowUpdate));
            }

            if (columnNames.Count == 1)
            {
                throw new ArgumentException($"'{nameof(rowUpdate)}' has no columns to update.", nameof(rowUpdate));
            }

            if (dataDictionary[idColumnName] == null)
            {
                throw new ArgumentException("Primary key column value must not be null.", nameof(rowUpdate));
            }

            using var helper = new CommandHelper(this, table, columnNames);
            var sql = this.ScriptBuilder.BuildUpdateScript(
                table,
                helper.GetParameterNames());

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

            var tableInfo = this.GetOrCreateTableInfo(tableName);
            var table = tableInfo.TableMold;

            //var table = this.Factory
            //    .CreateTable-Inspector(this.Connection, this.SchemaName, tableName)
            //    .GetTable();

            //var idColumnName = table.GetPrimaryKeySingleColumn(nameof(tableName)).Name;
            var idColumnName = tableInfo.PrimaryKeyColumnName;

            using var helper = new CommandHelper(this, table, new[] { idColumnName });

            var sql = this.ScriptBuilder.BuildDeleteByPrimaryKeyScript(
                table,
                helper.GetParameterNames().Single().Value);

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
