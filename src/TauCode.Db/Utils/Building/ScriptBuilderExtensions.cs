using System;
using System.Linq;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;

namespace TauCode.Db.Utils.Building
{
    public static class ScriptBuilderExtensions
    {
        public static string BuildDeleteRowByIdSql(this IScriptBuilder scriptBuilder, TableMold tableMold, out string paramName)
        {
            var colName = tableMold.GetSinglePrimaryKeyColumnName();

            return scriptBuilder.BuildDeleteRowByIdSql(tableMold.Name, colName, out paramName);
        }

        internal static string GetSinglePrimaryKeyColumnName(this TableMold tableMold)
        {
            if (tableMold == null)
            {
                throw new ArgumentNullException(nameof(tableMold));
            }

            try
            {
                var pkColumn = tableMold.Columns.SingleOrDefault(x => x.IsExplicitPrimaryKey());
                if (pkColumn != null)
                {
                    return pkColumn.Name;
                }
            }
            catch (Exception ex)
            {
                throw new ScriptBuildingException("An exception occured when tried to retrieve single-or-default explicit PRIMARY KEY column.", ex);
            }

            var pk = tableMold.PrimaryKey;
            if ((pk?.Columns?.Count ?? -1) != 1)
            {
                throw new ScriptBuildingException("Only tables having single-column primary key are supported.");
            }

            return pk.Columns.Single().Name;
        }
    }
}
