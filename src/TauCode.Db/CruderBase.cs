using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Data;
using TauCode.Db.Data;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public abstract class CruderBase : UtilityBase, ICruder
    {
        #region Nested

        protected class CommandHelper : IDisposable
        {
            #region Fields

            private readonly CruderBase _cruder;
            private IDbCommand _command;
            private bool _commandPrepared;

            private readonly IReadOnlyDictionary<string, ColumnMold> _columnsByColumnName;
            private readonly IReadOnlyDictionary<string, string> _parameterNamesByColumnNames;
            private readonly IReadOnlyDictionary<string, IParameterInfo> _parameterInfosByColumnNames;
            private readonly IReadOnlyDictionary<string, IDbDataParameter> _parametersByParameterNames;

            #endregion

            #region Constructor

            public CommandHelper(CruderBase cruder, TableMold table, IEnumerable<string> columnNames)
            {
                // todo checks

                _cruder = cruder;
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
                        table.Columns.Single(y =>
                            string.Equals(y.Name, x, StringComparison.InvariantCultureIgnoreCase)))
                    .ToDictionary(x => x.Name.ToLowerInvariant(), x => x);
            }

            private IReadOnlyDictionary<string, string> BuildParameterNamesByColumnNames()
            {
                return _columnsByColumnName
                    .ToDictionary(
                        x => x.Key,
                        x => $"p_{x.Key}");
            }

            private IReadOnlyDictionary<string, IParameterInfo> BuildParameterInfosByColumnNames()
            {
                return _columnsByColumnName
                    .ToDictionary(
                        x => x.Key,
                        x => _cruder.ColumnToParameterInfo(
                            x.Key,
                            x.Value.Type,
                            _parameterNamesByColumnNames));
            }

            private IReadOnlyDictionary<string, IDbDataParameter> BuildParametersByParameterNames()
            {
                return _parameterInfosByColumnNames
                    .ToDictionary(
                        x => x.Value.ParameterName,
                        x => this.CreateParameter(x.Value));
            }

            private IDbDataParameter CreateParameter(IParameterInfo parameterInfo)
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
                    var columnName = pair.Key;
                    var originalColumnValue = pair.Value;

                    var parameterInfo = _parameterInfosByColumnNames[columnName];
                    var parameterName = parameterInfo.ParameterName;
                    var parameter = _parametersByParameterNames[parameterName];

                    var columnValue = _cruder.TransformOriginalColumnValue(parameterInfo, originalColumnValue);

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
                this.ApplyValuesToCommand(values);
                var result = _command.ExecuteNonQuery();
                return result;
            }

            public IList<dynamic> FetchWithValues(object values)
            {
                this.ApplyValuesToCommand(values);
                var rows = DbUtils.GetCommandRows(_command);

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

        #endregion

        #region Fields

        private IScriptBuilderLab _scriptBuilderLab;

        #endregion

        #region Constructor

        protected CruderBase(IDbConnection connection)
            : base(connection, true, false)
        {
        }

        #endregion

        #region Protected

        protected virtual object TransformOriginalColumnValue(IParameterInfo parameterInfo, object originalColumnValue)
        {
            if (originalColumnValue == null)
            {
                return DBNull.Value;
            }

            object transformed;

            switch (parameterInfo.DbType)
            {
                case DbType.Guid: // todo: override for SQLite
                    if (originalColumnValue is Guid)
                    {
                        transformed = originalColumnValue;
                    }
                    else if (originalColumnValue is string stringValue)
                    {
                        transformed = new Guid(stringValue);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    break;

                case DbType.String:
                    if (originalColumnValue is string)
                    {
                        transformed = originalColumnValue;
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }

                    break;

                default:
                    throw new NotImplementedException();
            }

            return transformed;
        }

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

        protected virtual IParameterInfo ColumnToParameterInfo(
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

                case "uniqueidentifier":
                    dbType = DbType.Guid; // todo: override in SQLite
                    break;

                case "nvarchar":
                    dbType = DbType.String;
                    size = columnType.Size;
                    break;

                default:
                    throw new NotImplementedException();
            }

            IParameterInfo parameterInfo = new ParameterInfoImpl(parameterName, dbType, size, precision, scale);
            return parameterInfo;
        }

        #endregion

        #region ICruder Members

        public virtual IScriptBuilderLab ScriptBuilderLab =>
            _scriptBuilderLab ?? (_scriptBuilderLab = this.Factory.CreateScriptBuilderLab());

        public virtual void InsertRow(string tableName, object row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            this.InsertRows(tableName, new List<object> { row });
        }

        public virtual void InsertRows(string tableName, IReadOnlyList<object> rows)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            if (rows == null)
            {
                throw new ArgumentNullException(nameof(rows));
            }

            var table = this.Factory.CreateTableInspector(this.Connection, tableName).GetTable();

            if (rows.Count == 0)
            {
                return; // nothing to insert
            }

            using (var helper = new CommandHelper(this, table, this.ObjectToDataDictionary(rows[0]).Keys))
            {
                var sql = this.ScriptBuilderLab.BuildInsertScript(
                    table,
                    helper.GetParameterNames());

                helper.CommandText = sql;

                foreach (var row in rows)
                {
                    helper.ExecuteWithValues(row);
                }
            }
        }

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

            using (var helper = new CommandHelper(this, table, new[] { idColumnName }))
            {
                var sql = this.ScriptBuilderLab.BuildSelectByIdScript(table, helper.GetParameterNames().Single().Value);
                helper.CommandText = sql;
                var rows = helper.FetchWithValues(new Dictionary<string, object>
                {
                    {idColumnName, id}
                });

                return rows.SingleOrDefault();
            }
        }

        public virtual IList<dynamic> GetRows(string tableName)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            var table = this.Factory
                .CreateTableInspector(this.Connection, tableName)
                .GetTable();

            using (var command = this.Connection.CreateCommand())
            {
                var sql = this.ScriptBuilderLab.BuildSelectAllScript(table);
                command.CommandText = sql;
                var rows = DbUtils.GetCommandRows(command);
                return rows;
            }
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
            var columnNames = new List<string>(dataDictionary.Keys)
            {
                table.GetPrimaryKeyColumn().Name,
            };

            using (var helper = new CommandHelper(this, table, columnNames))
            {
                var sql = this.ScriptBuilderLab.BuildUpdateScript(
                    table,
                    helper.GetParameterNames());

                dataDictionary.Add(table.GetPrimaryKeyColumn().Name.ToLowerInvariant(), id);

                helper.CommandText = sql;
                var result = helper.ExecuteWithValues(dataDictionary);
                return result > 0;
            }
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

            using (var helper = new CommandHelper(this, table, new[] { idColumnName }))
            {
                var sql = this.ScriptBuilderLab.BuildDeleteScript(table, helper.GetParameterNames().Single().Value);
                helper.CommandText = sql;

                var result = helper.ExecuteWithValues(new Dictionary<string, object>
                {
                    {idColumnName, id}
                });

                return result > 0;
            }
        }

        #endregion
    }
}
