using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db.Model;

namespace TauCode.Db.SqlServer
{
    public class SqlServerTableInspector : /*SqlServerTableInspectorBase*/ TableInspectorBase
    {
        #region Constructor

        public SqlServerTableInspector(IDbConnection connection, string tableName)
            : base(SqlServerDialect.Instance, connection, tableName)
        {
        }

        #endregion

        #region Private

        private int GetTableObjectId()
        {
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText =
@"
SELECT
    T.object_id
FROM
    sys.tables T
WHERE
    T.name = @p_name
";
                command.AddParameterWithValue("p_name", this.TableName);
                var objectId = (int)command.ExecuteScalar();
                return objectId;
            }
        }

        #endregion

        #region Overridden

        protected override List<ColumnInfo> GetColumnInfos()
        {
            throw new System.NotImplementedException();
        }

        protected override ColumnMold ColumnInfoToColumnMold(ColumnInfo columnInfo)
        {
            throw new System.NotImplementedException();
        }

        protected override Dictionary<string, ColumnIdentityMold> GetIdentities()
        {
            var objectId = this.GetTableObjectId();

            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText =
@"
SELECT
    IC.name             Name,
    IC.seed_value       SeedValue,
    IC.increment_value  IncrementValue
FROM
    sys.identity_columns IC
WHERE
    IC.object_id = @p_objectId
";
                command.AddParameterWithValue("p_objectId", objectId);

                return UtilsHelper
                    .GetCommandRows(command)
                    .ToDictionary(
                        x => (string)x.Name,
                        x => new ColumnIdentityMold
                        {
                            Seed = ((object)x.SeedValue).ToString(),
                            Increment = ((object)x.IncrementValue).ToString(),
                        });
            }
        }

//        public override List<ForeignKeyMold> GetForeignKeyMolds()
//        {
//            using (var command = this.Connection.CreateCommand())
//            {
//                command.CommandText =
//                    @"
//SELECT
//    FK.[name]                   ForeignKeyName,
//    T.[name]                    TableName,
//    FKC.[constraint_column_id]  ColumnOrder,
//    C.[name]                    ColumnName,
//    TR.[name]                   ReferencedTableName,
//    CR.[name]                   ReferencedColumnName
//FROM
//    sys.foreign_keys FK
//INNER JOIN
//    sys.tables T
//ON
//    FK.[parent_object_id] = T.[object_id]
//INNER JOIN
//    sys.tables TR
//ON
//    FK.[referenced_object_id] = TR.[object_id]
//INNER JOIN
//    sys.foreign_key_columns FKC
//ON
//    FKC.[constraint_object_id] = FK.[object_id]
//INNER JOIN
//    sys.columns C
//ON
//    C.[object_id] = T.[object_id]
//    AND
//    C.[column_id] = FKC.[parent_column_id]
//INNER JOIN
//    sys.columns CR
//ON
//    CR.[object_id] = TR.[object_id]
//    AND
//    CR.[column_id] = FKC.[referenced_column_id]
//WHERE
//    T.[name] = @p_tableName
//";
//                command.AddParameterWithValue("p_tableName", this.TableName);


//                return UtilsHelper
//                    .GetCommandRows(command)
//                    .GroupBy(x => (string)x.ForeignKeyName)
//                    .Select(g => new ForeignKeyMold
//                    {
//                        Name = (string)g.First().ForeignKeyName,
//                        ReferencedTableName = (string)g.First().ReferencedTableName,
//                        ColumnNames = g
//                            .OrderBy(x => (int)x.ColumnOrder)
//                            .Select(x => (string)x.ColumnName)
//                            .ToList(),
//                        ReferencedColumnNames = g
//                            .OrderBy(x => (int)x.ColumnOrder)
//                            .Select(x => (string)x.ReferencedColumnName)
//                            .ToList(),
//                    })
//                    .OrderBy(x => x.Name)
//                    .ToList();
//            }
//        }

//        public override List<IndexMold> GetIndexMolds()
//        {
//            using (var command = this.Connection.CreateCommand())
//            {
//                // indexes list
//                command.CommandText =
//@"
//SELECT
//    I.[index_id]            IndexId,
//    I.[name]                IndexName,
//    I.[is_unique]           IndexIsUnique,
//    IC.[key_ordinal]        KeyOrdinal,
//    C.[name]                ColumnName,
//    IC.[is_descending_key]  IsDescendingKey
//FROM
//    sys.indexes I
//INNER JOIN
//    sys.index_columns IC
//ON
//    IC.[index_id] = I.[index_id]
//    AND
//    IC.[object_id] = i.[object_id]
//INNER JOIN
//    sys.columns C
//ON
//    C.[column_id] = IC.[column_id]
//    AND
//    C.[object_id] = IC.[object_id]
//INNER JOIN
//    sys.tables T
//ON
//    T.[object_id] = c.[object_id]
//WHERE
//    T.[name] = @p_tableName
//";
//                command.AddParameterWithValue("p_tableName", this.TableName);

//                return UtilsHelper
//                    .GetCommandRows(command)
//                    .GroupBy(x => (int)x.IndexId)
//                    .Select(g => new IndexMold
//                    {
//                        Name = (string)g.First().IndexName,
//                        IsUnique = (bool)g.First().IndexIsUnique,
//                        Columns = g
//                            .OrderBy(x => (int)x.KeyOrdinal)
//                            .Select(x => new IndexColumnMold
//                            {
//                                Name = (string)x.ColumnName,
//                                SortDirection = (bool)x.IsDescendingKey ? SortDirection.Descending : SortDirection.Ascending,
//                            })
//                            .ToList(),
//                    })
//                    .OrderBy(x => x.Name)
//                    .ToList();
//            }
//        }

        #endregion
    }
}
