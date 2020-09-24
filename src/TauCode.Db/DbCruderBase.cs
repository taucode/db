using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Data;
using TauCode.Db.Data;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;
using TauCode.Extensions;

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
            private readonly IReadOnlyDictionary<string, IDbParameterInfo> _parameterInfosByColumnNames;
            private readonly IReadOnlyDictionary<string, IDbDataParameter> _parametersByParameterNames;

            #endregion

            #region Constructor

            public CommandHelper(
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
                _parameterInfosByColumnNames = this.BuildParameterInfosByColumnNames();
                _parametersByParameterNames = this.BuildParametersByParameterNames();
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
                        var column = table.Columns.SingleOrDefault(y =>
                            string.Equals(y.Name, x, StringComparison.InvariantCultureIgnoreCase));

                        if (column == null)
                        {
                            throw new DbException($"Column not found: '{x}'.");
                        }

                        return column;
                    })
                    .ToDictionary(x => x.Name.ToLowerInvariant(), x => x);
            }

            private IReadOnlyDictionary<string, string> BuildParameterNamesByColumnNames()
            {
                return _columnsByColumnName
                    .ToDictionary(
                        x => x.Key,
                        x => $"p_{x.Key}");
            }

            private IReadOnlyDictionary<string, IDbParameterInfo> BuildParameterInfosByColumnNames()
            {
                var dictionary = new Dictionary<string, IDbParameterInfo>();

                foreach (var pair in _columnsByColumnName)
                {
                    var columnName = pair.Key;
                    var columnType = pair.Value.Type;
                    var parameterInfo =
                        _cruder.ColumnToParameterInfo(columnName, columnType, _parameterNamesByColumnNames);

                    if (parameterInfo == null)
                    {
                        throw new DbException($"Could not build parameter info for column '{columnName}'.");
                    }

                    dictionary.Add(columnName, parameterInfo);
                }

                return dictionary;
            }

            private IReadOnlyDictionary<string, IDbDataParameter> BuildParametersByParameterNames()
            {
                return _parameterInfosByColumnNames
                    .ToDictionary(
                        x => x.Value.ParameterName,
                        x => this.CreateParameter(x.Value));
            }

            private IDbDataParameter CreateParameter(IDbParameterInfo parameterInfo)
            {
                var parameter = _command.CreateParameter();
                parameter.ParameterName = parameterInfo.ParameterName;
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

                _command.Parameters.Add(parameter);
                return parameter;
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
                    var columnName = pair.Key.ToLowerInvariant();
                    var originalColumnValue = pair.Value;

                    if (!_parameterInfosByColumnNames.ContainsKey(columnName))
                    {
                        continue; // row has a value we will not insert into any table's column.
                    }

                    var parameterInfo = _parameterInfosByColumnNames[columnName];
                    var parameterName = parameterInfo.ParameterName;
                    var parameter = _parametersByParameterNames[parameterName];

                    var tableValuesConverter = _cruder.GetTableValuesConverter(_table.Name);
                    var dbValueConverter = tableValuesConverter.GetColumnConverter(columnName);
                    var columnValue = dbValueConverter.ToDbValue(originalColumnValue);

                    if (columnValue == null)
                    {
                        throw new DbException(
                            $"Could not transform value '{originalColumnValue}' of type '{originalColumnValue.GetType().FullName}'. Table name is '{_table.Name}'. Column name is '{columnName}'.");
                    }

                    if (columnValue is string stringColumnValue && parameterInfo.DbType.IsIn(
                        DbType.AnsiString,
                        DbType.AnsiStringFixedLength,
                        DbType.String,
                        DbType.StringFixedLength))
                    {
                        if (stringColumnValue.Length > parameter.Size && parameter.Size >= 0
                        ) // parameter.Size might be '-1', e.g. for type NVARCHAR(max)
                        {
                            throw _cruder.CreateTruncateException(columnName);
                        }
                    }
                    else if (columnValue is byte[] byteArray)
                    {
                        if (byteArray.Length > parameter.Size && parameter.Size >= 0
                        ) // parameter.Size might be '-1', e.g. for type VARBINARY(max)
                        {
                            throw _cruder.CreateTruncateException(columnName);
                        }
                    }

                    parameter.Value = columnValue;
                }
            }

            private void PrepareCommand()
            {
                _command.Prepare();
                _commandPrepared = true;
            }

            #endregion

            #region Public

            public string CommandText
            {
                get => _command.CommandText;
                set => _command.CommandText = value;
            }

            public int ExecuteWithValues(object values)
            {
                if (values == null)
                {
                    throw new ArgumentNullException(nameof(values));
                }

                this.ApplyValuesToCommand(values);

                var result = _command.ExecuteNonQuery();
                return result;
            }

            public IList<dynamic> FetchWithValues(object values)
            {
                this.ApplyValuesToCommand(values);
                var rows = DbTools.GetCommandRows(_command, _cruder.GetTableValuesConverter(_table.Name));

                return rows;
            }

            public IReadOnlyDictionary<string, string> GetParameterNames() => _parameterNamesByColumnNames;

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                _command?.Dispose();
                _command = null;
            }

            #endregion
        }

        protected DbException CreateTruncateException(string columnName)
        {
            return new DbException($"Data will be truncated for column '{columnName}'.");
        }

        #endregion

        #region Fields

        private IDbScriptBuilder _scriptBuilder;
        private readonly IDictionary<string, IDbTableValuesConverter> _tableValuesConverters;

        #endregion

        #region Constructor

        protected DbCruderBase(IDbConnection connection)
            : base(connection, true, false)
        {
            _tableValuesConverters = new Dictionary<string, IDbTableValuesConverter>();
        }

        #endregion

        #region Abstract

        protected abstract IDbValueConverter CreateDbValueConverter(ColumnMold column);

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

        protected virtual IDbParameterInfo ColumnToParameterInfo(
            string columnName,
            DbTypeMold columnType,
            IReadOnlyDictionary<string, string> parameterNameMappings)
        {
            DbType dbType;
            int? size = null;
            int? precision = null;
            int? scale = null;
            var parameterName = parameterNameMappings[columnName];

            var typeName = columnType.Name.ToLowerInvariant();

            switch (typeName)
            {
                case "int":
                    dbType = DbType.Int32;
                    break;

                case "tinyint":
                    dbType = DbType.Byte;
                    break;

                case "smallint":
                    dbType = DbType.Int16;
                    break;

                case "bigint":
                    dbType = DbType.Int64;
                    break;

                case "uniqueidentifier":
                    dbType = DbType.Guid;
                    break;

                case "char":
                    dbType = DbType.AnsiStringFixedLength;
                    size = columnType.Size;
                    break;

                case "varchar":
                    dbType = DbType.AnsiString;
                    size = columnType.Size;
                    break;

                case "nchar":
                    dbType = DbType.StringFixedLength;
                    size = columnType.Size;
                    break;

                case "nvarchar":
                    dbType = DbType.String;
                    size = columnType.Size;
                    break;

                case "date":
                    dbType = DbType.Date;
                    break;

                case "datetime":
                case "datetime2":
                    dbType = DbType.DateTime;
                    break;

                case "bit":
                    dbType = DbType.Boolean;
                    break;

                case "binary":
                case "varbinary":
                    dbType = DbType.Binary;
                    size = columnType.Size;
                    break;

                case "float":
                    dbType = DbType.Double;
                    break;

                case "real":
                    dbType = DbType.Single;
                    break;

                case "decimal":
                case "numeric":
                    dbType = DbType.Decimal;
                    precision = columnType.Precision;
                    scale = columnType.Scale;
                    break;

                default:
                    return null;
            }

            IDbParameterInfo parameterInfo = new DbParameterInfo(parameterName, dbType, size, precision, scale);
            return parameterInfo;
        }

        protected virtual IDbTableValuesConverter CreateTableValuesConverter(string tableName)
        {
            var tableInspector = this.Factory.CreateTableInspector(this.Connection, tableName);
            var table = tableInspector.GetTable();

            var dictionary = table.Columns
                .ToDictionary(
                    x => x.Name.ToLowerInvariant(),
                    this.CreateDbValueConverter);

            var tableValuesConverter = new DbTableValuesConverter(dictionary);
            return tableValuesConverter;
        }

        #endregion

        #region ICruder Members

        public virtual IDbScriptBuilder ScriptBuilder => _scriptBuilder ??= this.Factory.CreateScriptBuilder();

        public IDbTableValuesConverter GetTableValuesConverter(string tableName)
        {
            // todo: checks, lowercase, everywhere.
            var tableValuesConverter = _tableValuesConverters.GetOrDefault(tableName);
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
            IReadOnlyList<string> columnsToOmit = null)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            this.InsertRows(tableName, new List<object> { row }, columnsToOmit);
        }

        public virtual void InsertRows(
            string tableName,
            IReadOnlyList<object> rows,
            IReadOnlyList<string> columnsToOmit = null)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (rows == null)
            {
                throw new ArgumentNullException(nameof(rows));
            }

            if (columnsToOmit != null)
            {
                if (columnsToOmit.Any(string.IsNullOrWhiteSpace))
                {
                    throw new ArgumentException($"'{nameof(columnsToOmit)}' must not contain empty strings or nulls.");
                }
            }

            var table = this.Factory.CreateTableInspector(this.Connection, tableName).GetTable();

            if (rows.Count == 0)
            {
                return; // nothing to insert
            }

            columnsToOmit ??= new List<string>();

            var columnNames = this.ObjectToDataDictionary(rows[0]).Keys
                .Except(columnsToOmit, StringComparer.InvariantCultureIgnoreCase);

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
                .CreateTableInspector(this.Connection, tableName)
                .GetTable();

            var idColumnName = table.GetPrimaryKeyColumn().Name.ToLowerInvariant();

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
                .CreateTableInspector(this.Connection, tableName)
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
                .CreateTableInspector(this.Connection, tableName)
                .GetTable();

            var dataDictionary = this.ObjectToDataDictionary(rowUpdate);

            if (dataDictionary.Keys.Contains(table.GetPrimaryKeyColumn().Name,
                StringComparer.InvariantCultureIgnoreCase))
            {
                throw new DbException("Update object must not contain ID column.");
            }

            var columnNames = new List<string>(dataDictionary.Keys)
            {
                table.GetPrimaryKeyColumn().Name,
            };

            using var helper = new CommandHelper(this, table, columnNames);
            var sql = this.ScriptBuilder.BuildUpdateScript(
                table,
                helper.GetParameterNames());

            dataDictionary.Add(table.GetPrimaryKeyColumn().Name.ToLowerInvariant(), id);

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
                .CreateTableInspector(this.Connection, tableName)
                .GetTable();

            var idColumnName = table.GetPrimaryKeyColumn().Name.ToLowerInvariant();

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
