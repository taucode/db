using System;
using System.Collections.Generic;
using System.Text;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public static class DbScriptBuilderExtensions
    {
        public static string BuildCreateAllTablesScript(this IDbScriptBuilder scriptBuilder, IReadOnlyList<TableMold> tableMolds)
        {
            if (scriptBuilder == null)
            {
                throw new ArgumentNullException(nameof(scriptBuilder));
            }

            var sb = new StringBuilder();

            foreach (var tableMold in tableMolds)
            {
                sb.AppendLine($"/* Table: '{tableMold.Name}' */");
                var tableSql = scriptBuilder.BuildCreateTableScript(tableMold, true);
                sb.AppendLine(tableSql);
                sb.AppendLine();

                var indexes = scriptBuilder.Factory.GetDialect().GetCreatableIndexes(tableMold);

                foreach (var indexMold in indexes)
                {
                    sb.AppendLine($"/* Index: '{indexMold.Name}' on table '{tableMold.Name}' */");
                    var indexSql = scriptBuilder.BuildCreateIndexScript(indexMold);
                    sb.AppendLine(indexSql);
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }
    }
}
