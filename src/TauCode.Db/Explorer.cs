using System.Data;
using TauCode.Db.Model.Interfaces;

namespace TauCode.Db;

public abstract class Explorer : DataUtility, IExplorer
{
    #region ctor

    protected Explorer()
    {
    }

    protected Explorer(IDbConnection connection)
        : base(connection)
    {
    }

    #endregion

    #region IExplorer Members

    public abstract IReadOnlyList<string> GetSchemaNames();

    public IReadOnlyList<string> GetTableNames(string? schemaName)
    {
        var tableNames = this.GetTableNamesImpl(schemaName);
        return tableNames;
    }

    public abstract ITableMold GetTable(
        string? schemaName,
        string tableName,
        bool getConstraints = true,
        bool getIndexes = true);

    #endregion

    #region Abstract

    protected abstract IReadOnlyList<string> GetTableNamesImpl(string schemaName);

    #endregion
}