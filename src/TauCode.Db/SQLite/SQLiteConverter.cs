using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db.Model;

namespace TauCode.Db.SQLite
{
    public class SQLiteConverter : UtilityBase, IDbConverter
    {
        #region Constructor

        public SQLiteConverter()
            : base(null, false, true)
        {
        }

        #endregion

        #region Overridden

        public override IUtilityFactory Factory => SQLiteUtilityFactory.Instance;

        #endregion

        #region IDbConverter Members

        public DbTypeMold ConvertType(DbTypeMold originType, string originProviderName)
        {
            switch (originProviderName)
            {
                case DbProviderNames.SqlServer:
                    return this.ConvertTypeFromSqlServer(originType);

                default:
                    throw new NotSupportedException($"Conversion from '{originProviderName}' is not supported.");
            }
        }

        protected virtual DbTypeMold ConvertTypeFromSqlServer(DbTypeMold originType)
        {
            string typeName;
            var originTypeName = originType.Name.ToLowerInvariant();

            switch (originTypeName)
            {
                case "uniqueidentifier":
                    typeName = "UNIQUEIDENTIFIER";
                    break;

                case "char":
                case "nchar":
                case "varchar":
                case "nvarchar":
                case "text":
                case "ntext":
                    typeName = "TEXT";
                    break;

                case "bit":
                case "int":
                case "integer":
                case "tinyint":
                case "smallint":
                case "bigint":
                    typeName = "INTEGER";
                    break;

                case "binary":
                case "varbinary":
                case "image":
                    typeName = "BLOB";
                    break;

                case "double":
                case "float":
                case "real":
                    typeName = "REAL";
                    break;

                case "money":
                case "decimal":
                case "numeric":
                    typeName = "NUMERIC";
                    break;

                case "datetime":
                    typeName = "DATETIME";
                    break;

                default:
                    throw new NotSupportedException($"Conversion of type '{originTypeName}' is not supported.");
            }

            var result = new DbTypeMold
            {
                Name = typeName,
            };

            return result;
        }

        public ColumnIdentityMold ConvertIdentity(ColumnIdentityMold originIdentity, string originProviderName)
        {
            if (originIdentity == null)
            {
                return null;
            }

            throw new NotSupportedException("Conversion of identities is not supported.");
        }

        public string ConvertDefault(string originDefault, string originProviderName)
        {
            if (originDefault == null)
            {
                return null;
            }

            throw new NotSupportedException("Conversion of DEFAULT constraints is not supported.");
        }

        public PrimaryKeyMold ConvertPrimaryKey(TableMold originTable, string originProviderName)
        {
            return originTable.PrimaryKey.ClonePrimaryKey(false);
        }

        public IList<ForeignKeyMold> ConvertForeignKeys(TableMold originTable, string originProviderName)
        {
            return originTable
                .ForeignKeys
                .Select(x => x.CloneForeignKey(false))
                .ToList();
        }

        public IList<IndexMold> ConvertIndexes(TableMold originTable, string originProviderName)
        {
            var list = new List<IndexMold>();

            var pkColumns = originTable.PrimaryKey
                .Columns
                .Select(x => x.Name.ToLowerInvariant())
                .ToList();

            pkColumns.Sort();

            foreach (var index in originTable.Indexes)
            {
                var indexColumns = index
                    .Columns
                    .Select(x => x.Name.ToLowerInvariant())
                    .ToList();

                indexColumns.Sort();

                var needSkip = true;

                if (pkColumns.Count == indexColumns.Count)
                {
                    if (indexColumns.Where((t, i) => pkColumns[i] != t).Any())
                    {
                        needSkip = false;
                    }
                }

                if (needSkip)
                {
                    continue;
                }

                list.Add(index.CloneIndex(false));
            }

            return list;
        }

        public TableMold ConvertTable(TableMold originTable, string originProviderName)
        {
            var convertedTable = new TableMold
            {
                Name = originTable.Name,
                Columns = originTable
                    .Columns
                    .Select(x => new ColumnMold
                    {
                        Name = x.Name,
                        Type = this.ConvertType(x.Type, originProviderName),
                        IsNullable = x.IsNullable,
                        Identity = this.ConvertIdentity(x.Identity, originProviderName),
                        Default = this.ConvertDefault(x.Default, originProviderName),
                    })
                    .ToList(),
            };

            convertedTable.PrimaryKey = this.ConvertPrimaryKey(originTable, originProviderName);
            convertedTable.ForeignKeys = this.ConvertForeignKeys(originTable, originProviderName);
            convertedTable.Indexes = this.ConvertIndexes(originTable, originProviderName);

            return convertedTable;
        }

        public DbMold ConvertDb(DbMold originDb, IReadOnlyDictionary<string, string> options = null)
        {
            var result = new DbMold
            {
                DbProviderName = this.Factory.DbProviderName,
                Tables = originDb
                    .Tables
                    .Select(x => this.ConvertTable(x, originDb.DbProviderName))
                    .ToList(),
            };

            return result;
        }

        #endregion
    }
}
