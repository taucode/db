using System.Data;
using System.Linq;
using System.Text;
using TauCode.Db.Model;
using TauCode.Db.Utils.Dialects.SQLite;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.SQLite;

namespace TauCode.Db.Utils.Building.SQLite
{
    public class SQLiteScriptBuilder : ScriptBuilderBase
    {
        #region Constructor

        public SQLiteScriptBuilder() 
            : base(SQLiteDialect.Instance)
        {
        }

        #endregion

        #region Overridden

        protected override IDbInspector CreateDbInspector(IDbConnection connection)
        {
            return new SQLiteInspector(connection);
        }

        protected override string BuildIdentitySubSql(ColumnIdentityMold identity)
        {
            return "AUTOINCREMENT";
        }

        protected override void AddIndexCreationIntoTableCreationScript(TableMold table, StringBuilder sb)
        {
            // SQLite doesn't add index creation into table
        }

        protected override string BuildCreateTableSqlForCreateDbSql(TableMold table)
        {
            var sql = this.BuildCreateTableSql(table, true);
            return sql;
        }

        protected override string BuildAlterTableSqlForCreateDbSql(TableMold table, bool addComments)
        {
            var neededIndexes = table.Indexes
                .Where(x => x.Name != table.PrimaryKey.Name)
                .OrderBy(x => x.Name)
                .ToArray();

            var sb = new StringBuilder();

            // indexes
            if (neededIndexes.Length > 0)
            {
                sb.AppendLine();
                sb.AppendLine();
            }

            for (var i = 0; i < neededIndexes.Length; i++)
            {
                var index = neededIndexes[i];

                if (addComments)
                {
                    var uniqueWord = index.IsUnique ? "unique " : string.Empty;
                    sb.AppendLine($@"/* create {uniqueWord}index {index.Name}: {table.Name}({string.Join(", ", index.ColumnNames)}) */");
                }

                var indexSql = this.BuildIndexSql(table.Name, index);
                sb.Append(indexSql);

                if (i < neededIndexes.Length - 1)
                {
                    sb.AppendLine();
                    sb.AppendLine();
                }
            }

            return sb.ToString();
        }

        public override string BuildCreateTableSql(TableMold table, bool inline)
        {
            var identityColumn = table.Columns.SingleOrDefault(x => x.Identity != null);
            if (identityColumn != null)
            {
                identityColumn.Properties["force-inline-primary-key"] = "true";
            }

            return base.BuildCreateTableSql(table, inline);
        }

        #endregion
    }
}
