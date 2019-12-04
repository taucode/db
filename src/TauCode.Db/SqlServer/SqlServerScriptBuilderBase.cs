//using System;
//using System.Linq;
//using System.Text;
//using TauCode.Db.Model;

//namespace TauCode.Db.SqlServer
//{
//    public abstract class SqlServerScriptBuilderBase : ScriptBuilderBase
//    {
//        #region Constructor

//        protected SqlServerScriptBuilderBase(IDialect dialect)
//            : base(dialect)
//        {

//        }

//        #endregion

//        #region Overridden

//        protected override string BuildIdentitySubSql(ColumnIdentityMold identity)
//        {
//            return $"IDENTITY({identity.Seed}, {identity.Increment})";
//        }

//        protected override void AddIndexCreationIntoTableCreationScript(TableMold table, StringBuilder sb)
//        {
//            throw new NotSupportedException();
//        }

//        protected override string BuildCreateTableSqlForCreateDbSql(TableMold table)
//        {
//            var sql = this.BuildCreateTableSql(table, false);
//            return sql;
//        }

//        protected override string BuildAlterTableSqlForCreateDbSql(TableMold table, bool addComments)
//        {
//            var neededIndexes = table.Indexes
//                .Where(x => 
//                    x.Name != table.PrimaryKey?.Name)
//                .OrderBy(x => x.Name)
//                .ToArray();

//            var sb = new StringBuilder();

//            // indexes
//            if (neededIndexes.Length > 0)
//            {
//                sb.AppendLine();
//                sb.AppendLine();
//            }

//            for (var i = 0; i < neededIndexes.Length; i++)
//            {
//                var index = neededIndexes[i];

//                if (addComments)
//                {
//                    var uniqueWord = index.IsUnique ? "unique " : string.Empty;
//                    sb.AppendLine($@"/* create {uniqueWord}index {index.Name}: {table.Name}({string.Join(", ", index.Columns.Select(x => x.Name))}) */");
//                }

//                var indexSql = this.BuildIndexSql(table.Name, index);
//                sb.Append(indexSql);

//                if (i < neededIndexes.Length - 1)
//                {
//                    sb.AppendLine();
//                    sb.AppendLine();
//                }
//            }

//            // foreign keys
//            if (table.ForeignKeys.Count > 0)
//            {
//                sb.AppendLine();
//                sb.AppendLine();
//            }

//            var foreignKeys = table.ForeignKeys
//                .OrderBy(x => x.Name)
//                .ToList();

//            for (var i = 0; i < foreignKeys.Count; i++)
//            {
//                var foreignKey = foreignKeys[i];

//                if (addComments)
//                {
//                    var from = $"{table.Name}({string.Join(", ", foreignKey.ColumnNames)})";
//                    var to = $"{foreignKey.ReferencedTableName}({string.Join(", ", foreignKey.ReferencedColumnNames)})";
//                    sb.AppendLine($@"/* create foreign key {foreignKey.Name}: {from} -> {to} */");
//                }

//                var foreignKeySql = this.BuildForeignKeySql(table.Name, foreignKey);
//                sb.Append(foreignKeySql);

//                if (i < foreignKeys.Count - 1)
//                {
//                    sb.AppendLine();
//                    sb.AppendLine();
//                }
//            }

//            return sb.ToString();
//        }

//        protected override string BuildTypeDefinitionSql(DbTypeMold type)
//        {
//            if (type.Size.HasValue && type.Size.Value == -1)
//            {
//                var decoratedName = this.Dialect.DecorateIdentifier(
//                    DbIdentifierType.Type,
//                    type.Name,
//                    this.CurrentOpeningIdentifierDelimiter);

//                return $"{decoratedName}(max)";
//            }
//            else
//            {
//                return base.BuildTypeDefinitionSql(type);
//            }
//        }

//        #endregion
//    }
//}
