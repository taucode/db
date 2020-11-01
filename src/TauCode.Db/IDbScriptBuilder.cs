using System;
using System.Collections.Generic;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public interface IDbScriptBuilder : IDbUtility
    {
        string SchemaName { get; }
        char? CurrentOpeningIdentifierDelimiter { get; set; }
        string BuildCreateTableScript(TableMold table, bool includeConstraints);
        string BuildCreateIndexScript(IndexMold index);
        string BuildDropTableScript(string tableName);
        string BuildInsertScript(TableMold table, IReadOnlyDictionary<string, string> columnToParameterMappings);
        string BuildUpdateScript(TableMold table, IReadOnlyDictionary<string, string> columnToParameterMappings);
        string BuildSelectByPrimaryKeyScript(TableMold table, string pkColumnParameterName, Func<string, bool> columnSelector = null);
        string BuildSelectAllScript(TableMold table, Func<string, bool> columnSelector = null);
        string BuildDeleteByPrimaryKeyScript(TableMold table, string pkColumnParameterName);
        string BuildDeleteScript(string tableName);
    }
}
