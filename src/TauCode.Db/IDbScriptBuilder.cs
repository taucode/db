﻿using System.Collections.Generic;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public interface IDbScriptBuilder : IDbUtility
    {
        char? CurrentOpeningIdentifierDelimiter { get; set; }
        string BuildCreateTableScript(TableMold table, bool includeConstraints);
        string BuildCreateIndexScript(IndexMold index);
        string BuildDropTableScript(string tableName);
        string BuildInsertScript(TableMold table, IReadOnlyDictionary<string, string> columnToParameterMappings);
        string BuildUpdateScript(TableMold table, IReadOnlyDictionary<string, string> columnToParameterMappings);
        string BuildSelectByIdScript(TableMold table, string idParameterName);
        string BuildSelectAllScript(TableMold table);
        string BuildDeleteByIdScript(TableMold table, string idParameterName);
        string BuildDeleteScript(string tableName);
    }
}
