using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Dialects;

namespace TauCode.Db.Utils
{
    internal static class UtilsHelper
    {
        internal static void AddParameterWithValue(
            this IDbCommand command,
            string parameterName,
            object parameterValue)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            command.Parameters.Add(parameter);
        }

        internal static int? GetDbValueAsInt(object dbValue)
        {
            if (dbValue == DBNull.Value)
            {
                return null;
            }

            return int.Parse(dbValue.ToString());
        }

        internal static T? GetDbValue<T>(object dbValue) where T : struct
        {
            if (dbValue == DBNull.Value)
            {
                return null;
            }

            return (T)dbValue;
        }

        internal static string DecorateColumnsOverComma(
            this ScriptBuilderBase scriptBuilder,
            List<string> columnNames,
            char? delimiter)
        {
            var sb = new StringBuilder();

            for (var i = 0; i < columnNames.Count; i++)
            {
                var decoratedColumnName =
                    scriptBuilder.Dialect.DecorateIdentifier(DbIdentifierType.Column, columnNames[i], delimiter);
                sb.Append(decoratedColumnName);

                if (i < columnNames.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            return sb.ToString();
        }

        internal static string DecorateIndexColumnsOverComma(
            this ScriptBuilderBase scriptBuilder,
            List<IndexColumnMold> columns,
            char? delimiter)
        {
            var sbIndexColumns = new StringBuilder();
            for (var i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                sbIndexColumns.Append(scriptBuilder.Dialect.DecorateIdentifier(
                    DbIdentifierType.Column,
                    column.Name,
                    delimiter));

                string sortDirection;
                switch (column.SortDirection)
                {
                    case SortDirection.Ascending:
                        sortDirection = "ASC";
                        break;

                    case SortDirection.Descending:
                        sortDirection = "DESC";
                        break;

                    default:
                        throw new ScriptBuildingException($"Invalid sort direction: '{column.SortDirection}'.");
                }

                sbIndexColumns.Append(" ");
                sbIndexColumns.Append(sortDirection);

                if (i < columns.Count - 1)
                {
                    sbIndexColumns.Append(", ");
                }
            }

            return sbIndexColumns.ToString();
        }

        internal static string ByteArrayToHex(byte[] bytes)
        {
            var sb = new StringBuilder();
            sb.Append("0x");

            foreach (var b in bytes)
            {
                sb.AppendFormat("{0:x00}", b);
            }

            return sb.ToString();
        }

        internal static ColumnMold GetColumn(this TableMold table, string columnName)
        {
            return table.Columns.Single(x =>
                string.Equals(x.Name, columnName, StringComparison.InvariantCultureIgnoreCase));
        }

        internal static SyntaxAnalyzerException CreateInternalSyntaxAnalyzerErrorException()
        {
            return new SyntaxAnalyzerException("Syntax analyzer error.");
        }

        internal static HashSet<T> ToMyHashSet<T>(this IEnumerable<T> collection)
        {
            return new HashSet<T>(collection);
        }
    }
}
