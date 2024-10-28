using System.Text;
using TauCode.Db.Model.Interfaces;
using TauCode.Db.Model.Molds;

namespace TauCode.Db;

public abstract class ScriptBuilder : Utility, IScriptBuilder
{
    #region Protected

    protected static bool AcceptField(string columnName) => true;

    #endregion

    #region IScriptBuilder Members

    public string BuildSelectAllRowsScript(ITableMold tableMold, Func<string, bool>? fieldSelector = null)
    {
        var sb = new StringBuilder();
        var fullTableName = tableMold.SchemaName == null ? tableMold.Name : $"{tableMold.SchemaName}.{tableMold.Name}";

        var tableAlias = "T";

        sb.Append("SELECT");
        sb.AppendLine();

        fieldSelector ??= AcceptField;
        var wantedColumns = tableMold
            .Columns
            .Where(x => fieldSelector(x.Name))
            .ToList();

        for (var i = 0; i < wantedColumns.Count; i++)
        {
            var wantedColumn = wantedColumns[i];


            sb.Append("    ");
            sb.Append(tableAlias);
            sb.Append(".");
            sb.Append(wantedColumn.Name);

            if (i < wantedColumns.Count - 1)
            {
                sb.AppendLine(",");
            }
        }

        sb.AppendLine();
        sb.Append("FROM");
        sb.AppendLine();

        sb.Append("    ");
        sb.Append(fullTableName);
        sb.Append(" ");
        sb.Append(tableAlias);

        var sql = sb.ToString();
        return sql;
    }

    public string BuildInsertScript(ITableMold tableMold, Func<string, bool>? fieldSelector, out IDictionary<string, IParameterMold> parameterMapping)
    {
        var sb = new StringBuilder();
        var fullTableName = tableMold.SchemaName == null ? tableMold.Name : $"{tableMold.SchemaName}.{tableMold.Name}";

        sb.Append(@"INSERT INTO ");
        sb.Append(fullTableName);
        sb.AppendLine("(");

        fieldSelector ??= AcceptField;

        var wantedColumns = tableMold
            .Columns
            .Where(x => fieldSelector(x.Name))
            .ToList();

        parameterMapping = new Dictionary<string, IParameterMold>();

        for (var i = 0; i < wantedColumns.Count; i++)
        {
            var wantedColumn = wantedColumns[i];

            var paramName = $"p_{wantedColumn.Name}";

            var parameterMold = new ParameterMold
            {
                Name = paramName,
                Column = wantedColumn,
            };

            parameterMapping.Add(wantedColumn.Name, parameterMold);

            sb.Append("    ");
            sb.Append(wantedColumn.Name);

            if (i < wantedColumns.Count - 1)
            {
                sb.AppendLine(",");
            }
        }

        sb.AppendLine();
        sb.AppendLine(")");
        sb.AppendLine("VALUES(");

        for (var i = 0; i < wantedColumns.Count; i++)
        {
            var wantedColumnName = wantedColumns[i].Name;
            var paramName = parameterMapping[wantedColumnName].Name;

            sb.Append("    @");
            sb.Append(paramName);

            if (i < wantedColumns.Count - 1)
            {
                sb.AppendLine(",");
            }
        }

        sb.AppendLine();
        sb.AppendLine(")");

        var sql = sb.ToString();

        return sql;
    }

    #endregion
}