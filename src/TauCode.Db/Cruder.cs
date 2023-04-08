using System.Data;
using TauCode.Db.Model.Interfaces;

namespace TauCode.Db;

public abstract class Cruder : DataUtility, ICruder
{
    #region Fields

    private IExplorer? _explorer;
    private IScriptBuilder _scriptBuilder = null!;

    #endregion

    #region ctor

    protected Cruder()
    {
    }

    protected Cruder(IDbConnection? connection)
        : base(connection)
    {
    }

    #endregion

    #region Polymorph

    protected virtual IExplorer CreateExplorer() => this.Factory.CreateExplorer();

    protected virtual IScriptBuilder CreateScriptBuilder() => this.Factory.CreateScriptBuilder();

    protected virtual void OnBeforeInsertRows(
        ITableMold tableMold,
        IRowSet rows,
        Func<string, bool>? fieldSelector = null)
    {
        // idle
    }

    protected virtual void OnAfterInsertRows(
        ITableMold tableMold,
        IRowSet rows,
        Func<string, bool>? fieldSelector = null)
    {
        // idle
    }

    #endregion

    #region ICruder Members

    public IValueConverter ValueConverter { get; set; } = new DefaultValueConverter();

    public IExplorer Explorer => _explorer ??= this.CreateExplorer();

    public IScriptBuilder ScriptBuilder => _scriptBuilder ??= this.CreateScriptBuilder();

    public IRowSet GetAllRows(
        string? schemaName,
        string tableName,
        Func<string, bool>? fieldSelector = null)
    {
        this.Explorer.Connection = this.Connection;
        this.Explorer.Connection = this.Connection;
        var tableMold = this.Explorer.GetTable(schemaName, tableName); // todo: consider caching table molds

        var selectSql = this.ScriptBuilder.BuildSelectAllRowsScript(tableMold, fieldSelector);
        using var command = this.Connection!.CreateCommand();
        command.CommandText = selectSql;

        var rowSet = command.ReadRowsFromCommand();
        return rowSet;
    }

    public void InsertRows(
        string? schemaName,
        string tableName,
        IRowSet rows,
        Func<string, bool>? fieldSelector = null)
    {
        // todo: check connection is not null

        this.Explorer.Connection = this.Connection;
        var tableMold = this.Explorer.GetTable(schemaName, tableName); // todo: consider caching table molds

        var insertSql = this.ScriptBuilder.BuildInsertScript(tableMold, fieldSelector, out var mapping);

        // todo: must depend on 'rows'. e.g. if rows' FieldNames contain identity column, then execute IDENTITY_INSERT on. if doesn't contain, then don't execute.
        this.OnBeforeInsertRows(tableMold, rows, fieldSelector);

        using var command = this.Connection!.CreateCommand();
        command.CommandText = insertSql;

        foreach (var pair in mapping)
        {
            var columnName = pair.Key;
            var parameterMold = pair.Value;

            var columnMold = parameterMold.Column!;

            var parameter = command.CreateParameter();

            parameter.ParameterName = parameterMold.Name;
            if (columnMold.Type.Size.HasValue)
            {
                parameter.Size = columnMold.Type.Size.Value;
            }

            if (columnMold.Type.Precision.HasValue)
            {
                parameter.Precision = (byte)columnMold.Type.Precision.Value;
            }

            if (columnMold.Type.Scale.HasValue)
            {
                parameter.Scale = (byte)columnMold.Type.Scale.Value;
            }

            this.TuneParameter(parameter, pair.Value.Column!);
            parameterMold.Parameter = parameter;

            command.Parameters.Add(parameter);
        }

        command.Prepare();

        foreach (var row in rows)
        {
            foreach (var pair in mapping)
            {
                var columnName = pair.Key;
                var parameterMold = pair.Value;
                var columnMold = parameterMold.Column!;

                var value = row[columnName];

                if (this.ValueConverter != null)
                {
                    value = this.ValueConverter.ConvertToDb(schemaName, tableName, columnMold, value);
                }

                parameterMold.Parameter!.Value = value;
            }

            command.ExecuteNonQuery();
        }

        this.OnAfterInsertRows(tableMold, rows, fieldSelector);
    }

    protected abstract void TuneParameter(IDbDataParameter parameter, IColumnMold columnMold);

    #endregion
}
