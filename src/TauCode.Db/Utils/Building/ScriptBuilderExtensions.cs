using System;
using System.Linq;
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

            var pk = tableMold.PrimaryKey;
            if ((pk?.ColumnNames?.Count ?? -1) != 1)
            {
                throw new InvalidOperationException("Only tables having single-column primary key are supported.");
            }

            return pk.ColumnNames.Single();
        }
    }
}
