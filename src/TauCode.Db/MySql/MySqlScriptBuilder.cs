using System;
using System.Data;
using System.Linq;
using System.Text;
using TauCode.Db.Model;
using TauCode.Utils.Extensions;

namespace TauCode.Db.MySql
{
    public class MySqlScriptBuilder : ScriptBuilderBase
    {
        #region Constructor

        public MySqlScriptBuilder()
            : base(MySqlDialect.Instance)
        {
        }

        #endregion

        #region Overridden

        protected override IDbInspector CreateDbInspector(IDbConnection connection)
        {
            return new MySqlInspector(connection);
        }

        protected override string BuildIdentitySubSql(ColumnIdentityMold identity)
        {
            return "AUTO_INCREMENT";
        }

        public override string BuildPrimaryKeySql(string tableName, PrimaryKeyMold primaryKey)
        {
            if (tableName == null)
            {
                var decoratedColumnNames = this.DecorateIndexColumnsOverComma(
                    primaryKey.Columns,
                    this.CurrentOpeningIdentifierDelimiter);

                return $"PRIMARY KEY({decoratedColumnNames})";
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        protected override string BuildColumnPropertiesSubSql(ColumnMold column)
        {
            var sb = new StringBuilder();

            var characterSet = column.Properties.GetOrDefault("CharacterSet");
            if (characterSet != null)
            {
                sb.Append($"CHARACTER SET {characterSet}");
            }

            var collation = column.Properties.GetOrDefault("Collation");
            if (collation != null)
            {
                if (sb.Length > 0)
                {
                    sb.Append(" ");
                }

                sb.Append($"COLLATE {collation}");
            }

            return sb.ToString();
        }

        protected override void AddIndexCreationIntoTableCreationScript(TableMold table, StringBuilder sb)
        {
            var neededIndexes = table.Indexes
                .Where(x => x.Name != "PRIMARY")
                .OrderBy(x => x.Name)
                .ToList();

            if (neededIndexes.Count > 0)
            {
                sb.AppendLine(",");

                for (var i = 0; i < neededIndexes.Count; i++)
                {
                    var index = neededIndexes[i];
                    var sql = this.IndexToInlineTableKeySql(index);
                    sb.Append(sql);
                    if (i < neededIndexes.Count - 1)
                    {
                        sb.AppendLine(",");
                    }
                }
            }
        }

        protected override string BuildCreateTableSqlForCreateDbSql(TableMold table)
        {
            return this.BuildCreateTableSql(table, true);
        }

        protected override string BuildAlterTableSqlForCreateDbSql(TableMold table, bool addComments)
        {
            return string.Empty;
        }

        #endregion

        #region Private

        private string IndexToInlineTableKeySql(IndexMold index)
        {
            var sb = new StringBuilder();

            var uniqueWord = index.IsUnique ? "UNIQUE " : string.Empty;

            var decoratedIndexName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Index,
                index.Name,
                this.CurrentOpeningIdentifierDelimiter);

            sb.Append($"    {uniqueWord}KEY ");
            sb.Append(decoratedIndexName);

            var columnNames = this.DecorateIndexColumnsOverComma(index.Columns, this.CurrentOpeningIdentifierDelimiter);
            sb.Append($"({columnNames})");

            return sb.ToString();
        }

        #endregion
    }
}
