using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Data;
using TauCode.Db.Data;
using TauCode.Db.Model;

namespace TauCode.Db
{
    // todo: cache scripts. YES! NNNNOOOOOOooooooooooooo!
    public abstract class CruderBase : UtilityBase, ICruder
    {
        #region Nested

        protected class CommandHelper : IDisposable
        {
            private readonly CruderBase _cruder;

            //private readonly TableMold _table;
            //private readonly IList<string> _columnNames;
            private IDbCommand _command;
            private bool _commandPrepared;

            private readonly IReadOnlyDictionary<string, ColumnMold> _columnsByColumnName;
            private readonly IReadOnlyDictionary<string, string> _parameterNamesByColumnNames;
            private readonly IReadOnlyDictionary<string, IParameterInfo> _parameterInfosByColumnNames;
            private readonly IReadOnlyDictionary<string, IDbDataParameter> _parametersByParameterNames;

            public CommandHelper(CruderBase cruder, TableMold table, IEnumerable<string> columnNames)
            {
                // todo checks

                _cruder = cruder;
                //_table = table;
                //_columnNames = columnNames
                //    .Select(x => x.ToLowerInvariant())
                //    .ToList();

                _command = _cruder.Connection.CreateCommand();

                _columnsByColumnName = this.BuildColumnsByColumnName(table, columnNames);
                _parameterNamesByColumnNames = this.BuildParameterNamesByColumnNames();
                _parameterInfosByColumnNames = this.BuildParameterInfosByColumnNames();
                _parametersByParameterNames = this.BuildParametersByParameterNames();
            }

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

                //return _columnNames
                //    .Select(x => _table.Columns.Single(y => y.Name == x))
                //    .ToDictionary(
                //            x => x.Name.ToLowerInvariant(),
                //            x => $"p_{x.Name}");


                //throw new NotImplementedException();
                //return _table.Columns.ToDictionary(
                //    x => x.Name.ToLowerInvariant(),
                //    x => $"p_{x.Name}");
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

            public string CommandText
            {
                get => _command.CommandText;
                set => _command.CommandText = value;
            }

            public int ExecuteWithValues(object values)
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

                var result = _command.ExecuteNonQuery();
                return result;
            }

            private void PrepareCommand()
            {
                _command.Prepare();
                _commandPrepared = true;
            }

            public IReadOnlyDictionary<string, string> GetParameterNames() => _parameterNamesByColumnNames;

            public void Dispose()
            {
                _command?.Dispose();
                _command = null;
            }
        }

        #endregion

        #region Fields

        //private readonly IDbConnection _connection;
        //private IScriptBuilder _scriptBuilder;
        //private IDbInspector _dbInspector;

        private IScriptBuilderLab _scriptBuilderLab;
        private IDbInspector _dbInspector;


        //private readonly IDictionary<string, CrudCommandBuilder> _insertCommands; // todo: need this?

        #endregion

        #region Constructor

        protected CruderBase(IDbConnection connection)
            : base(connection, true, false)
        {
            //_insertCommands = new Dictionary<string, CrudCommandBuilder>();
        }

        #endregion

        #region Polymorph

        // todo: move to factory.
        //protected abstract string ExpectedDbConnectionTypeFullName { get; }

        //protected abstract IUtilityFactory GetFactoryImpl();

        //protected abstract IScriptBuilder CreateScriptBuilder();

        //protected abstract IDbInspector CreateDbInspector();

        #endregion

        #region Protected

        //protected IDbConnection GetSafeConnection()
        //{
        //    throw new NotImplementedException();
        //    //if (_connection.GetType().FullName == this.ExpectedDbConnectionTypeFullName)
        //    //{
        //    //    return _connection;
        //    //}

        //    //throw new TypeMismatchException(
        //    //    $"Expected DB connection type is '{this.ExpectedDbConnectionTypeFullName}', but an instance of '{_connection.GetType().FullName}' was provided.");
        //}

        protected IDbInspector DbInspector =>
            _dbInspector ?? (_dbInspector = this.Factory.CreateDbInspector(this.Connection));

        //protected virtual CrudCommandBuilder GetInserter(string tableName)
        //{
        //    if (tableName == null)
        //    {
        //        throw new ArgumentNullException(nameof(tableName));
        //    }

        //    tableName = tableName.ToLowerInvariant();

        //    var inserter = _insertCommands.GetOrDefault(tableName);
        //    if (inserter == null)
        //    {
        //        var table = this.Factory.CreateTableInspector(this.Connection, tableName).GetTable();
        //        var insertableColumns = table
        //            .Columns
        //            .Where(x => x.Identity == null)
        //            .Select(x => x.Name);

        //        inserter = new CrudCommandBuilder(this.Connection, insertableColumns);
        //        inserter.CommandText =
        //            this.ScriptBuilderLab.BuildInsertScript(table, inserter.GetColumnToParameterMappings());

        //        _insertCommands.Add(tableName, inserter);
        //    }

        //    return inserter;
        //}

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

        #endregion

        #region ICruder Members

        //public IDbInspector DbInspector => _dbInspector ?? (_dbInspector = this.CreateDbInspector());

        //public IScriptBuilder ScriptBuilder => _scriptBuilder ?? (_scriptBuilder = this.CreateScriptBuilder());

        public IScriptBuilderLab ScriptBuilderLab =>
            _scriptBuilderLab ?? (_scriptBuilderLab = this.Factory.CreateScriptBuilderLab());

        public void InsertRow(string tableName, object row)
        {
            //if (tableName == null)
            //{
            //    throw new ArgumentNullException(nameof(tableName));
            //}

            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            this.InsertRows(tableName, new List<object> {row});

            //var inserter = this.GetInserter(tableName);
            //var dataDictionary = this.ObjectToDataDictionary(row);

            //inserter.SetData(dataDictionary);
            //inserter.GetCommand().ExecuteNonQuery();


            //IDictionary<string, object> dictionary;

            //if (row is IDictionary<string, object> dictionaryParam)
            //{
            //    dictionary = dictionaryParam;
            //}
            //else if (row is DynamicRow dynamicRow)
            //{
            //    dictionary = dynamicRow.ToDictionary();
            //}
            //else
            //{
            //    dictionary = new ValueDictionary(row);
            //}

            //var connection = this.GetSafeConnection();

            //var tableInspector = this.DbInspector.GetTableInspector(tableName);
            //var tableMold = tableInspector.GetTableMold();

            //var script = this.ScriptBuilder.BuildParameterizedInsertSql(
            //    tableMold,
            //    out var parameterMapping,
            //    columnsToInclude: dictionary.Keys.ToArray(),
            //    indent: 4);

            //using (var command = connection.CreateCommand())
            //{
            //    command.CommandText = script;

            //    foreach (var columnName in parameterMapping.Keys)
            //    {
            //        var parameterName = parameterMapping[columnName];
            //        var parameterValue = dictionary[columnName] ?? DBNull.Value;

            //        command.AddParameterWithValue(parameterName, parameterValue);
            //    }

            //    command.ExecuteNonQuery();
            //}
        }

        public void InsertRows(string tableName, IReadOnlyList<object> rows)
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

            //var helper = new CommandHelper(this, table, rows[]);

            //var rowValuesDictionary = this.ObjectToDataDictionary(rows[0]); // origin, sample to compare with

            using (var helper = new CommandHelper(this, table, this.ObjectToDataDictionary(rows[0]).Keys))
            {
                var sql = this.ScriptBuilderLab.BuildInsertScript(
                    table,
                    helper.GetParameterNames()
                    //parameterInfoDictionary.ToDictionary(
                    //    pair => pair.Key,
                    //    pair => pair.Value.ParameterName)

                );

                helper.CommandText = sql;

                foreach (var row in rows)
                {
                    helper.ExecuteWithValues(row);
                    //var rowDictionary = this.ObjectToDataDictionary(row); // little overhead for row #0
                    //foreach (var pair in rowDictionary)
                    //{
                    //    var columnName = pair.Key;
                    //    var originalColumnValue = pair.Value;

                    //    var parameterInfo = parameterInfoDictionary[columnName];
                    //    var parameterName = parameterInfo.ParameterName;
                    //    var parameter = parameterDictionary[parameterName];

                    //    var columnValue = this.TransformOriginalColumnValue(parameterInfo, originalColumnValue);

                    //    parameter.Value = columnValue;
                    //}

                    //command.ExecuteNonQuery();
                }
            }

            //var parameterInfoDictionary = this.BuildParameterInfoDictionary(table, rowValuesDictionary.Keys);



            //IDictionary<string, IDbDataParameter> parameterDictionary = new Dictionary<string, IDbDataParameter>();

            //using (var command = this.Connection.CreateCommand())
            //{
            //    command.CommandText = sql;
            //    foreach (var pair in parameterInfoDictionary)
            //    {
            //        var parameterInfo = pair.Value;

            //        var parameter = command.CreateParameter();
            //        parameter.ParameterName = parameterInfo.ParameterName;
            //        parameter.DbType = parameterInfo.DbType;

            //        if (parameterInfo.Size.HasValue)
            //        {
            //            parameter.Size = parameterInfo.Size.Value;
            //        }

            //        if (parameterInfo.Precision.HasValue)
            //        {
            //            parameter.Precision = (byte) parameterInfo.Precision.Value;
            //        }

            //        if (parameterInfo.Scale.HasValue)
            //        {
            //            parameter.Scale = (byte) parameterInfo.Scale.Value;
            //        }

            //        command.Parameters.Add(parameter);
            //        parameterDictionary.Add(parameter.ParameterName, parameter);
            //    }

            //    command.Prepare();

            //    foreach (var row in rows)
            //    {
            //        var rowDictionary = this.ObjectToDataDictionary(row); // little overhead for row #0
            //        foreach (var pair in rowDictionary)
            //        {
            //            var columnName = pair.Key;
            //            var originalColumnValue = pair.Value;

            //            var parameterInfo = parameterInfoDictionary[columnName];
            //            var parameterName = parameterInfo.ParameterName;
            //            var parameter = parameterDictionary[parameterName];

            //            var columnValue = this.TransformOriginalColumnValue(parameterInfo, originalColumnValue);

            //            parameter.Value = columnValue;
            //        }

            //        command.ExecuteNonQuery();
            //    }
            //}
        }

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
                    throw new ArgumentOutOfRangeException();
            }

            return transformed;
        }

        //private IDictionary<string, IParameterInfo> BuildParameterInfoDictionary(
        //    TableMold table,
        //    ICollection<string> columnNames)
        //{
        //    var dictionary = new Dictionary<string, IParameterInfo>();

        //    foreach (var columnName in columnNames)
        //    {
        //        var column = table.Columns.Single(x => x.Name == columnName);
        //        var parameterInfo = this.ColumnToParameterInfo(column);

        //        dictionary.Add(columnName, parameterInfo);
        //    }

        //    return dictionary;
        //}

        protected virtual IParameterInfo ColumnToParameterInfo(
            string columnName,
            //ColumnMold column,
            DbTypeMold columnType,
            IReadOnlyDictionary<string, string> parameterNameMappings)
        {
            DbType dbType;
            int? size = null;
            int? precision = null;
            int? scale = null;
            var parameterName = parameterNameMappings[columnName];
            //var parameterName = $"p_{column.Name}";

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

        public bool DeleteRow(string tableName, object id)
        {
            throw new NotImplementedException();
            //if (tableName == null)
            //{
            //    throw new ArgumentNullException(nameof(tableName));
            //}

            //if (id == null)
            //{
            //    throw new ArgumentNullException(nameof(id));
            //}

            //var tableInspector = this.DbInspector.GetTableInspector(tableName);
            //var tableMold = tableInspector.GetTableMold();
            //var connection = this.GetSafeConnection();


            //using (var command = connection.CreateCommand())
            //{
            //    command.CommandText = this.ScriptBuilder.BuildDeleteRowByIdSql(tableMold, out var paramName);
            //    command.AddParameterWithValue(paramName, id);

            //    var deletedCount = command.ExecuteNonQuery();
            //    return deletedCount == 1;
            //}
        }

        public dynamic GetRow(string tableName, object id)
        {
            throw new NotImplementedException();
            //if (tableName == null)
            //{
            //    throw new ArgumentNullException(nameof(tableName));
            //}

            //if (id == null)
            //{
            //    throw new ArgumentNullException(nameof(id));
            //}

            //var tableInspector = this.DbInspector.GetTableInspector(tableName);
            //var tableMold = tableInspector.GetTableMold();
            //var connection = this.GetSafeConnection();

            //using (var command = connection.CreateCommand())
            //{
            //    command.CommandText = this.ScriptBuilder.BuildSelectRowByIdSql(tableMold, out var paramName);
            //    command.AddParameterWithValue(paramName, id);

            //    var rows = UtilsHelper.GetCommandRows(command);
            //    return rows.SingleOrDefault();
            //}
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

            throw new NotImplementedException();

            //IDictionary<string, object> dictionary;

            //if (rowUpdate is IDictionary<string, object> dictionaryParam)
            //{
            //    dictionary = dictionaryParam;
            //}
            //else if (rowUpdate is DynamicRow dynamicRow)
            //{
            //    dictionary = dynamicRow.ToDictionary();
            //}
            //else
            //{
            //    dictionary = new ValueDictionary(rowUpdate);
            //}

            //var tableInspector = this.DbInspector.GetTableInspector(tableName);
            //var tableMold = tableInspector.GetTableMold();
            //var connection = this.GetSafeConnection();

            //var pkColumnName = tableMold.GetSinglePrimaryKeyColumnName();

            //var script = this.ScriptBuilder.BuildUpdateRowByIdSql(
            //    tableMold.Name,
            //    pkColumnName,
            //    dictionary.Keys.ToArray(),
            //    out var parameterMapping);

            //using (var command = connection.CreateCommand())
            //{
            //    command.CommandText = script;

            //    foreach (var columnName in parameterMapping.Keys)
            //    {
            //        var paramName = parameterMapping[columnName];
            //        object paramValue;

            //        if (columnName == pkColumnName)
            //        {
            //            paramValue = id;
            //        }
            //        else
            //        {
            //            paramValue = dictionary[columnName] ?? DBNull.Value;
            //        }

            //        command.AddParameterWithValue(paramName, paramValue);
            //    }

            //    var rowsAffected = command.ExecuteNonQuery();
            //    return rowsAffected > 0; // actually, should always be 0 or 1.
            //}
        }

        //public void Reset()
        //{
        //    _insertCommands.Clear();
        //}

        #endregion
    }
}
