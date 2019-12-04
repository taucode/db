using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Data;
using TauCode.Db.Data;
using TauCode.Db.Model;
using TauCode.Utils.Extensions;

namespace TauCode.Db
{
    // todo: cache scripts. YES!
    public abstract class CruderBase : UtilityBase, ICruder
    {
        #region Fields

        //private readonly IDbConnection _connection;
        //private IScriptBuilder _scriptBuilder;
        //private IDbInspector _dbInspector;

        private IScriptBuilderLab _scriptBuilderLab;
        private IDbInspector _dbInspector;


        private readonly IDictionary<string, CrudCommandBuilder> _insertCommands; // todo: need this?

        #endregion

        #region Constructor

        protected CruderBase(IDbConnection connection)
            : base(connection, true, false)
        {
            _insertCommands = new Dictionary<string, CrudCommandBuilder>();
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

        protected virtual CrudCommandBuilder GetInserter(string tableName)
        {
            if (tableName == null)
            {
                throw new ArgumentNullException(nameof(tableName));
            }

            tableName = tableName.ToLowerInvariant();

            var inserter = _insertCommands.GetOrDefault(tableName);
            if (inserter == null)
            {
                var table = this.Factory.CreateTableInspector(this.Connection, tableName).GetTable();
                var insertableColumns = table
                    .Columns
                    .Where(x => x.Identity == null)
                    .Select(x => x.Name);

                inserter = new CrudCommandBuilder(this.Connection, insertableColumns);
                inserter.CommandText =
                    this.ScriptBuilderLab.BuildInsertScript(table, inserter.GetColumnToParameterMappings());

                _insertCommands.Add(tableName, inserter);
            }

            return inserter;
        }

        protected virtual IDictionary<string, object> RowToDataDictionary(object row)
        {
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

            var rowValuesDictionary = this.RowToDataDictionary(rows[0]); // origin, sample to compare with
            var parameterInfoDictionary = this.BuildParameterInfoDictionary(table, rowValuesDictionary.Keys);

            var sql = this.ScriptBuilderLab.BuildInsertScript(
                table,
                parameterInfoDictionary.ToDictionary(
                    pair => pair.Key,
                    pair => pair.Value.ParameterName));

            IDictionary<string, IDbDataParameter> parameterDictionary = new Dictionary<string, IDbDataParameter>();

            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText = sql;
                foreach (var pair in parameterInfoDictionary)
                {
                    var parameterInfo = pair.Value;

                    var parameter = command.CreateParameter();
                    parameter.ParameterName = parameterInfo.ParameterName;
                    parameter.DbType = parameterInfo.DbType;

                    if (parameterInfo.Size.HasValue)
                    {
                        parameter.Size = parameterInfo.Size.Value;
                    }

                    if (parameterInfo.Precision.HasValue)
                    {
                        parameter.Precision = (byte) parameterInfo.Precision.Value;
                    }

                    if (parameterInfo.Scale.HasValue)
                    {
                        parameter.Scale = (byte) parameterInfo.Scale.Value;
                    }

                    command.Parameters.Add(parameter);
                    parameterDictionary.Add(parameter.ParameterName, parameter);
                }

                command.Prepare();

                foreach (var row in rows)
                {
                    var rowDictionary = this.RowToDataDictionary(row); // little overhead for row #0
                    foreach (var pair in rowDictionary)
                    {
                        var columnName = pair.Key;
                        var originalColumnValue = pair.Value;

                        var parameterInfo = parameterInfoDictionary[columnName];
                        var parameterName = parameterInfo.ParameterName;
                        var parameter = parameterDictionary[parameterName];

                        var columnValue = this.TransformOriginalColumnValue(parameterInfo, originalColumnValue);

                        parameter.Value = columnValue;
                    }

                    command.ExecuteNonQuery();
                }
            }
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

        private IDictionary<string, IParameterInfo> BuildParameterInfoDictionary(TableMold table,
            ICollection<string> columnNames)
        {
            var dictionary = new Dictionary<string, IParameterInfo>();

            foreach (var columnName in columnNames)
            {
                var column = table.Columns.Single(x => x.Name == columnName);
                var parameterInfo = this.ColumnToParameterInfo(column);

                dictionary.Add(columnName, parameterInfo);
            }

            return dictionary;
        }

        protected virtual IParameterInfo ColumnToParameterInfo(ColumnMold column)
        {
            DbType dbType;
            int? size = null;
            int? precision = null;
            int? scale = null;
            var parameterName = $"p_{column.Name}";

            var typeName = column.Type.Name.ToLowerInvariant();

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
                    size = column.Type.Size;
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
            throw new NotImplementedException();

            //if (tableName == null)
            //{
            //    throw new ArgumentNullException(nameof(tableName));
            //}

            //if (rowUpdate == null)
            //{
            //    throw new ArgumentNullException(nameof(rowUpdate));
            //}

            //if (id == null)
            //{
            //    throw new ArgumentNullException(nameof(id));
            //}

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

        public void Reset()
        {
            _insertCommands.Clear();
        }

        #endregion
    }
}
