﻿using System.Collections.Generic;
using TauCode.Db.Model;

namespace TauCode.Db
{
    // todo: get rid of lab.
    public interface IScriptBuilderLab : IUtility
    {
        char? CurrentOpeningIdentifierDelimiter { get; set; }
        string BuildCreateTableScript(TableMold table, bool includeConstraints);
        string BuildInsertScript(TableMold table, IReadOnlyDictionary<string, string> columnToParameterMappings);
        string BuildUpdateScript(TableMold table, IReadOnlyDictionary<string, string> columnToParameterMappings);
        string BuildSelectByIdScript(TableMold table, string idParameterName);
        string BuildSelectAllScript(TableMold table);
        string BuildDeleteScript(TableMold table, string idParameterName);
    }
}