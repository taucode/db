using System.Data;
using System.Text.RegularExpressions;
using TauCode.Db.Collections;
using TauCode.Db.Model.Interfaces;

namespace TauCode.Db;

// todo: consider renaming to "DbExtensions"
public static class DbTools
{
    // todo: consider async variants

    public static IRowSet ReadRowsFromCommand(this IDbCommand command)
    {
        using var reader = command.ExecuteReader();

        var fieldCount = reader.FieldCount;
        var fieldNames = new string[fieldCount];
        for (var i = 0; i < fieldCount; i++)
        {
            fieldNames[i] = reader.GetName(i);
        }

        var rowSet = new ListRowSet(fieldNames);

        while (reader.Read())
        {
            var row = rowSet.Add();

            for (var i = 0; i < fieldCount; i++)
            {
                var fieldName = fieldNames[i];
                var value = reader.GetValue(i);
                row[fieldName] = value;
            }
        }

        return rowSet;
    }

    public static void ExecuteSql(
        this IDbConnection connection,
        string sql)
    {
        if (connection == null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    public static IDbConnection GetOpenConnection(this IDataUtility utility)
    {
        if (utility == null)
        {
            throw new ArgumentNullException(nameof(utility));
        }

        if (utility.Connection == null)
        {
            throw new InvalidOperationException($"'{nameof(IDataUtility.Connection)}' must not be null.");
        }

        if ((utility.Connection.State & ConnectionState.Open) == 0)
        {
            throw new InvalidOperationException($"'{nameof(IDataUtility.Connection)}' must be open.");
        }

        return utility.Connection;
    }

    public static IList<string> SplitScriptByComments(string script)
    {
        var statements = Regex.Split(script, @"/\*.*?\*/", RegexOptions.Singleline)
            .Select(x => x.Trim())
            .Where(x => x != string.Empty)
            .ToArray();

        return statements;
    }

    public static IList<IDictionary<string, object>> MaterializeRowSet(this IRowSet rowSet)
    {
        return rowSet.Cast<IDictionary<string, object>>().ToList();
    }

    public static IDictionary<string, IList<ITableMold>> LoadSchema(
        this IExplorer explorer,
        bool getConstraints = true,
        bool getIndexes = true)
    {
        // todo: schemaNames will be null or empty array for SQLite?

        var dictionary = new Dictionary<string, IList<ITableMold>>();

        var schemaNames = explorer.GetSchemaNames();
        foreach (var schemaName in schemaNames)
        {
            var tableNames = explorer.GetTableNames(schemaName);
            var tableMolds = new List<ITableMold>();

            dictionary.Add(schemaName, tableMolds);

            foreach (var tableName in tableNames)
            {
                var tableMold = explorer.GetTable(schemaName, tableName, getConstraints, getIndexes);
                tableMolds.Add(tableMold);
            }
        }

        return dictionary;
    }
}