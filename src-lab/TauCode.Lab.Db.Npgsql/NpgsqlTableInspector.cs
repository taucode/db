using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Model;

// todo clean
namespace TauCode.Lab.Db.Npgsql
{
    public class NpgsqlTableInspector : DbTableInspectorBase
    {
        #region Constructor

        public NpgsqlTableInspector(IDbConnection connection, string schema, string tableName)
            : base(
                connection,
                schema ?? NpgsqlInspector.DefaultSchema,
                tableName)
        {

        }

        #endregion

        #region Private

//        private int GetTableObjectId()
//        {
//            using var command = this.Connection.CreateCommand();
//            command.CommandText =
//                @"
//SELECT
//    T.object_id
//FROM
//    sys.tables T
//WHERE
//    T.name = @p_name
//";
//            command.AddParameterWithValue("p_name", this.TableName);

//            var objectResult = command.ExecuteScalar();

//            if (objectResult == null)
//            {
//                throw DbTools.CreateTableNotFoundException(this.TableName);
//            }
                
//            var objectId = (int)objectResult;
//            return objectId;
//        }

        private static bool ParseBoolean(object value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value is bool b)
            {
                return b;
            }

            if (value is string s)
            {
                if (s.ToLower() == "yes")
                {
                    return true;
                }
                else if (s.ToLower() == "no")
                {
                    return false;
                }
                else
                {
                    throw new ArgumentException($"Could not parse value '{s}' as boolean.");
                }
            }

            throw new ArgumentException($"Could not parse value '{value}' of type '{value.GetType().FullName}' as boolean.");
        }

        private static int? GetDbValueAsInt(object dbValue)
        {
            if (dbValue == null)
            {
                return null;
            }

            return int.Parse(dbValue.ToString());
        }

        #endregion

        #region Overridden

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactory.Instance;

        protected override List<ColumnInfo> GetColumnInfos()
        {
            using var command = this.Connection.CreateCommand();
            command.CommandText =
                @"
SELECT
    C.column_name               ColumnName,
    C.is_nullable               IsNullable,
    C.data_type                 DataType,
    C.character_maximum_length  MaxLen,
    C.numeric_precision         NumericPrecision,
    C.numeric_scale             NumericScale
FROM
    information_schema.columns C
WHERE
    C.table_name = @p_tableName
ORDER BY
    C.ordinal_position
";

            command.AddParameterWithValue("p_tableName", this.TableName);

            var columnInfos = DbTools
                .GetCommandRows(command)
                .Select(x => new ColumnInfo
                {
                    Name = x.ColumnName,
                    TypeName = x.DataType,
                    IsNullable = ParseBoolean(x.IsNullable),
                    Size = GetDbValueAsInt(x.MaxLen),
                    Precision = GetDbValueAsInt(x.NumericPrecision),
                    Scale = GetDbValueAsInt(x.NumericScale),
                })
                .ToList();

            return columnInfos;
        }

        protected override ColumnMold ColumnInfoToColumnMold(ColumnInfo columnInfo)
        {
            var column = new ColumnMold
            {
                Name = columnInfo.Name,
                Type = new DbTypeMold
                {
                    Name = columnInfo.TypeName,
                    Size = columnInfo.Size,
                    Precision = columnInfo.Precision,
                    Scale = columnInfo.Scale,
                },
                IsNullable = columnInfo.IsNullable,
            };

            return column;


            //var column = new ColumnMold
            //{
            //    Name = columnInfo.Name,
            //    Type = this.Factory.GetDialect().ResolveType(
            //        columnInfo.TypeName,
            //        columnInfo.Size,
            //        columnInfo.Precision,
            //        columnInfo.Scale),
            //    IsNullable = columnInfo.IsNullable,
            //};

            //return column;
        }

        protected override Dictionary<string, ColumnIdentityMold> GetIdentities()
        {
            //var objectId = this.GetTableObjectId();

            using var command = this.Connection.CreateCommand();
            command.CommandText =
@"
SELECT 
    S.column_name           ColumnName, 
    S.is_identity           IsIdentity,
    S.identity_start        SeedValue,
    S.identity_increment    IncrementValue
FROM 
    information_schema.columns S
WHERE 
    S.table_name = @p_tableName AND
    S.is_identity = 'YES'
";
            command.AddParameterWithValue("p_tableName", this.TableName);

            //command.AddParameterWithValue("p_objectId", objectId);

            return DbTools
                .GetCommandRows(command)
                .ToDictionary(
                    x => (string)x.Name,
                    x => new ColumnIdentityMold
                    {
                        Seed = ((object)x.SeedValue).ToString(),
                        Increment = ((object)x.IncrementValue).ToString(),
                    });
        }

        public override PrimaryKeyMold GetPrimaryKey()
        {
            using var command = this.Connection.CreateCommand();

            command.CommandText =
@"
SELECT
    TC.constraint_name    ConstraintName,
    KCU.column_name       ColumnName,
    KCU.ordinal_position  OrdinalPosition
FROM
    information_schema.table_constraints TC
INNER JOIN
    information_schema.key_column_usage KCU
ON
    KCU.table_name = TC.table_name AND
    KCU.constraint_name = TC.constraint_name
WHERE
    TC.table_name = @p_tableName
    AND
    TC.constraint_type = 'PRIMARY KEY'
ORDER BY
    OrdinalPosition
";
            command.AddParameterWithValue("p_tableName", this.TableName);

            
            var rows = DbTools.GetCommandRows(command);
            if (rows.Count == 0)
            {
                return null;
            }

            var pkName = rows[0].ConstraintName;
            var pk = new PrimaryKeyMold
            {
                Name = pkName,
                Columns = rows
                    .Select(x => new IndexColumnMold
                    {
                        Name = (string)x.ColumnName,
                        SortDirection = SortDirection.Ascending,
                    })
                    .ToList(),
            };

            return pk;


//            // get PK name
//            command.CommandText =
//@"
//SELECT
//    TC.constraint_name
//FROM
//    information_schema.table_constraints TC
//WHERE
//    TC.table_name = @p_tableName
//    AND
//    TC.constraint_type = 'PRIMARY KEY'";

//            var parameter = command.CreateParameter();
//            parameter.ParameterName = "p_tableName";
//            parameter.Value = this.TableName;
//            command.Parameters.Add(parameter);

//            var constraintName = (string)command.ExecuteScalar();

//            if (constraintName == null)
//            {
//                return null;
//            }

//            // get PK columns
//            command.Parameters.Clear();
//            command.CommandText =
//@"
//SELECT
//    S.column_name       ColumnName,
//    S.ordinal_position  OrdinalPosition
//FROM
//    information_schema.key_column_usage S
//WHERE
//    S.table_name = @p_tableName AND
//    S.constraint_name = @p_constraintName
//ORDER BY
//    OrdinalPosition
//";
//            command.AddParameterWithValue("p_constraintName", constraintName);
//            command.AddParameterWithValue("p_tableName", this.TableName);


//            var columns = DbTools
//                .GetCommandRows(command)
//                .Select(x => new IndexColumnMold
//                {
//                    Name = (string)x.ColumnName,
//                    SortDirection = SortDirection.Ascending,
//                })
//                .ToList();

//            var primaryKeyMold = new PrimaryKeyMold
//            {
//                Name = constraintName,
//                Columns = columns,
//            };

//            return primaryKeyMold;
        }

        public override IReadOnlyList<ForeignKeyMold> GetForeignKeys()
        {
            using var command = this.Connection.CreateCommand();

            command.CommandText =
                @"
SELECT
    TC.costraint_name    ConstraintName
FROM
    information_schema.table_constraints TC
WHERE
    TC.constraint_schema = @p_schema AND
    TC.table_schema = @p_schema AND
    TC.table_name = @p_tableName    
";
            command.AddParameterWithValue("@p_schema", this.Schema);
            command.AddParameterWithValue("@p_tableName", this.TableName);

            throw new NotImplementedException();


            command.CommandText =
@"
SELECT
    FK.[name]                   ForeignKeyName,
    T.[name]                    TableName,
    FKC.[constraint_column_id]  ColumnOrder,
    C.[name]                    ColumnName,
    TR.[name]                   ReferencedTableName,
    CR.[name]                   ReferencedColumnName
FROM
    sys.foreign_keys FK
INNER JOIN
    sys.tables T
ON
    FK.[parent_object_id] = T.[object_id]
INNER JOIN
    sys.tables TR
ON
    FK.[referenced_object_id] = TR.[object_id]
INNER JOIN
    sys.foreign_key_columns FKC
ON
    FKC.[constraint_object_id] = FK.[object_id]
INNER JOIN
    sys.columns C
ON
    C.[object_id] = T.[object_id]
    AND
    C.[column_id] = FKC.[parent_column_id]
INNER JOIN
    sys.columns CR
ON
    CR.[object_id] = TR.[object_id]
    AND
    CR.[column_id] = FKC.[referenced_column_id]
WHERE
    T.[name] = @p_tableName
";
            command.AddParameterWithValue("p_tableName", this.TableName);

            return DbTools
                .GetCommandRows(command)
                .GroupBy(x => (string)x.ForeignKeyName)
                .Select(g => new ForeignKeyMold
                {
                    Name = (string)g.First().ForeignKeyName,
                    ReferencedTableName = (string)g.First().ReferencedTableName,
                    ColumnNames = g
                        .OrderBy(x => (int)x.ColumnOrder)
                        .Select(x => (string)x.ColumnName)
                        .ToList(),
                    ReferencedColumnNames = g
                        .OrderBy(x => (int)x.ColumnOrder)
                        .Select(x => (string)x.ReferencedColumnName)
                        .ToList(),
                })
                .OrderBy(x => x.Name)
                .ToList();
        }

        public override IReadOnlyList<IndexMold> GetIndexes()
        {
            using var command = this.Connection.CreateCommand();
            // indexes list
            command.CommandText =
                @"
SELECT
    I.[index_id]            IndexId,
    I.[name]                IndexName,
    I.[is_unique]           IndexIsUnique,
    IC.[key_ordinal]        KeyOrdinal,
    C.[name]                ColumnName,
    IC.[is_descending_key]  IsDescendingKey
FROM
    sys.indexes I
INNER JOIN
    sys.index_columns IC
ON
    IC.[index_id] = I.[index_id]
    AND
    IC.[object_id] = i.[object_id]
INNER JOIN
    sys.columns C
ON
    C.[column_id] = IC.[column_id]
    AND
    C.[object_id] = IC.[object_id]
INNER JOIN
    sys.tables T
ON
    T.[object_id] = c.[object_id]
WHERE
    T.[name] = @p_tableName
";
            command.AddParameterWithValue("p_tableName", this.TableName);

            return DbTools
                .GetCommandRows(command)
                .GroupBy(x => (int)x.IndexId)
                .Select(g => new IndexMold
                {
                    Name = (string)g.First().IndexName,
                    TableName = this.TableName,
                    Columns = g
                        .OrderBy(x => (int)x.KeyOrdinal)
                        .Select(x => new IndexColumnMold
                        {
                            Name = (string)x.ColumnName,
                            SortDirection = (bool)x.IsDescendingKey ? SortDirection.Descending : SortDirection.Ascending,
                        })
                        .ToList(),
                    IsUnique = (bool)g.First().IndexIsUnique,
                })
                .OrderBy(x => x.Name)
                .ToList();
        }

        #endregion
    }
}
