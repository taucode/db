using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Model;

// todo clean
namespace TauCode.Lab.Db.MySql
{
    public class MySqlTableInspector : DbTableInspectorBase
    {
        #region Constructor

        public MySqlTableInspector(IDbConnection connection, string tableName)
            : base(
                connection,
                null,
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

        public override IDbUtilityFactory Factory => MySqlUtilityFactory.Instance;

        protected override List<ColumnInfo> GetColumnInfos()
        {
            var dbName = this.Connection.GetDatabaseName();

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
    C.table_name = @p_tableName AND
    C.table_schema = @p_dbName    
ORDER BY
    C.ordinal_position
";

            command.AddParameterWithValue("p_tableName", this.TableName);
            command.AddParameterWithValue("p_dbName", dbName);

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
        }

        protected override Dictionary<string, ColumnIdentityMold> GetIdentities()
        {
            var dbName = this.Connection.GetDatabaseName();

            using var command = this.Connection.CreateCommand();

            // find out if table supports auto_increment

            command.CommandText =
@"
SELECT
    T.auto_increment    AutoIncrement
FROM
    information_schema.tables T
WHERE
    T.table_name = @p_tableName
    AND
    T.table_type = @p_tableType
    AND
    T.table_schema = @p_dbName
";
            command.AddParameterWithValue("p_tableName", this.TableName);
            command.AddParameterWithValue("p_tableType", MySqlTools.TableTypeForTable);
            command.AddParameterWithValue("p_dbName", dbName);

            var rows = DbTools.GetCommandRows(command);

            int? autoIncrement = (int?)rows.Single().AutoIncrement;

            if (autoIncrement.HasValue)
            {
                command.Parameters.Clear();

                command.CommandText = @"
SELECT 
    S.column_name ColumnName
FROM 
    information_schema.columns S
WHERE
    S.table_schema = @p_dbName
    AND
    S.table_name = @p_tableName
    AND
    S.extra like '%auto_increment%'
";

                command.AddParameterWithValue("p_dbName", dbName);
                command.AddParameterWithValue("p_tableName", this.TableName);

                var rows2 = DbTools.GetCommandRows(command);
                var columnName = (string)rows2.Single().ColumnName;

                return new Dictionary<string, ColumnIdentityMold>
                {
                    {
                        columnName,
                        new ColumnIdentityMold
                        {
                            Seed = autoIncrement.Value.ToString(),
                            Increment = (1).ToString(),
                        }
                    },
                };
            }
            else
            {
                return new Dictionary<string, ColumnIdentityMold>();
            }

            // todo clean below
            //command.AddParameterWithValue("p_objectId", objectId);

            //return DbTools
            //    .GetCommandRows(command)
            //    .ToDictionary(
            //        x => (string)x.Name,
            //        x => new ColumnIdentityMold
            //        {
            //            Seed = ((object)x.SeedValue).ToString(),
            //            Increment = ((object)x.IncrementValue).ToString(),
            //        });
        }

        public override PrimaryKeyMold GetPrimaryKey()
        {
            return MySqlTools.LoadPrimaryKey(this.Connection, this.TableName);

            // todo clean below
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
            var dbName = this.Connection.GetDatabaseName();

            using var command = this.Connection.CreateCommand();

            // get constraint names
            command.CommandText =
@"
SELECT
    TC.constraint_name                  ConstraintName,
    KCU.column_name                     ColumnName,
    KCU.referenced_table_name           ReferencedTableName,
    KCU.referenced_column_name          ReferencedColumnName,
    KCU.ordinal_position                OrdinalPosition,
    KCU.position_in_unique_constraint   PositionInUniqueConstraint
FROM
    information_schema.table_constraints TC
INNER JOIN
    information_schema.key_column_usage KCU
ON
    TC.constraint_name = KCU.constraint_name
WHERE
    TC.constraint_schema = @p_dbName AND
    TC.table_schema = @p_dbName AND

    KCU.constraint_schema = @p_dbName AND
    KCU.table_schema = @p_dbName AND
    KCU.referenced_table_schema = @p_dbName AND

    TC.table_name = @p_tableName AND
    TC.constraint_type = 'FOREIGN KEY'
ORDER BY
    TC.constraint_name,
    KCU.ordinal_position
";
            command.AddParameterWithValue("@p_dbName", dbName);
            command.AddParameterWithValue("@p_tableName", this.TableName);

            var rows = DbTools.GetCommandRows(command);

            return rows
                .GroupBy(x => (string)x.ConstraintName)
                .Select(x => new ForeignKeyMold
                {
                    Name = x.Key,
                    ColumnNames = x.Select(y => (string)y.ColumnName).ToList(),
                    ReferencedTableName = (string)x.First().ReferencedTableName,
                    ReferencedColumnNames = x.Select(y => (string)y.ReferencedColumnName).ToList(),

                })
                .ToList();


            //            var fkNames = rows
            //                .Select(x => (string) x.ConstraintName)
            //                .ToList();

            //            var list = new List<ForeignKeyMold>();

            //            command.Parameters.Clear();
            //            command.CommandText = @"
            //SELECT
            //    KCU.column_name
            //";

            //            foreach (var fkName in fkNames)
            //            {


            //                var fk = new ForeignKeyMold
            //                {
            //                    Name = fkName,
            //                    ColumnNames = columnNames,
            //                    ReferencedTableName = referencedTableName,
            //                    ReferencedColumnNames = referencedColumnNames,
            //                };

            //                list.Add(fk);
            //            }

            //            return list;


            //var fkNames = rows
            //    .Select(x => (string)x.ConstraintName)
            //    .ToList();

            //return fkNames
            //    .Select(this.LoadForeignKey)
            //    .ToList();

            // todo clean
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

        private ForeignKeyMold LoadForeignKey(string fkName)
        {
            // get referenced table name
            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    CCU.table_name TableName
FROM
    information_schema.constraint_column_usage CCU
WHERE
    CCU.table_schema = @p_schema
    AND
    CCU.constraint_schema = @p_schema
    AND
    CCU.constraint_name = @p_fkName
";
            command.AddParameterWithValue("p_schema", this.Schema);
            command.AddParameterWithValue("p_fkName", fkName);

            var referencedTableName = DbTools.GetCommandRows(command)
                .Select(x => (string)x.TableName)
                .Distinct()
                .Single();

            // get referenced table PK
            var referencedTablePk = MySqlTools.LoadPrimaryKey(this.Connection, referencedTableName);

            // get foreign key columns

            command.Parameters.Clear();

            command.CommandText = @"
select
    KCU.column_name 					ColumnName,
    KCU.ordinal_position 				OrdinalPosition,
    KCU.position_in_unique_constraint 	PositionInUniqueConstraint
from
    information_schema.key_column_usage KCU	
where
    KCU.constraint_schema = @p_schema and
    KCU.table_schema = @p_schema and
    KCU.table_name = @p_tableName and
    KCU.constraint_name = @p_fkName
order by
    KCU.ordinal_position
";

            command.AddParameterWithValue("p_schema", this.Schema);
            command.AddParameterWithValue("p_tableName", this.TableName);
            command.AddParameterWithValue("p_fkName", fkName);

            var rows = DbTools.GetCommandRows(command);

            var columnNames = new List<string>();
            var referencedColumnNames = new List<string>();

            foreach (var row in rows)
            {
                var columnName = (string)row.ColumnName;
                var ordinalPosition = (int)row.OrdinalPosition;
                var positionInUniqueConstraint = (int)row.PositionInUniqueConstraint;

                columnNames.Add(columnName);

                var referencedColumnName = referencedTablePk.Columns[positionInUniqueConstraint - 1].Name;

                referencedColumnNames.Add(referencedColumnName);
            }

            var fk = new ForeignKeyMold
            {
                Name = fkName,
                ReferencedTableName = referencedTableName,
                ColumnNames = columnNames,
                ReferencedColumnNames = referencedColumnNames,
            };

            return fk;
        }

        public override IReadOnlyList<IndexMold> GetIndexes()
        {
            var dbName = this.Connection.GetDatabaseName();

            using var command = this.Connection.CreateCommand();

            command.CommandText = @"
SELECT
    S.index_name    IndexName,
    S.non_unique    NonUnique,
    S.column_name   ColumnName,
    S.seq_in_index  SeqInIndex
FROM
    information_schema.statistics S
WHERE
    S.table_schema = @p_dbName
    AND
    S.index_schema = @p_dbName
    AND
    S.table_name = @p_tableName
";
            command.AddParameterWithValue("p_dbName", dbName);
            command.AddParameterWithValue("p_tableName", this.TableName);

            var rows = DbTools.GetCommandRows(command);

            return rows
                .GroupBy(x => (string)x.IndexName)
                .Select(x => new IndexMold
                {
                    Name = x.Key,
                    TableName = this.TableName,
                    Columns = x
                        .OrderBy(y => (int)y.SeqInIndex)
                        .Select(y => new IndexColumnMold
                        {
                            Name = (string)y.ColumnName,
                            SortDirection = SortDirection.Ascending,
                        })
                        .ToList(),
                    IsUnique = x.First().NonUnique.ToString() == "0",
                })
                .ToList();

            //throw new NotImplementedException();

            //return rows
            //    .Select(x => this.BuildIndexMold((string)x.IndexName, (string)x.IndexDef))
            //    .ToList();



            //            throw new NotImplementedException();


            //            // indexes list
            //            command.CommandText =
            //                @"
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
            //            command.AddParameterWithValue("p_tableName", this.TableName);

            //            return DbTools
            //                .GetCommandRows(command)
            //                .GroupBy(x => (int)x.IndexId)
            //                .Select(g => new IndexMold
            //                {
            //                    Name = (string)g.First().IndexName,
            //                    TableName = this.TableName,
            //                    Columns = g
            //                        .OrderBy(x => (int)x.KeyOrdinal)
            //                        .Select(x => new IndexColumnMold
            //                        {
            //                            Name = (string)x.ColumnName,
            //                            SortDirection = (bool)x.IsDescendingKey ? SortDirection.Descending : SortDirection.Ascending,
            //                        })
            //                        .ToList(),
            //                    IsUnique = (bool)g.First().IndexIsUnique,
            //                })
            //                .OrderBy(x => x.Name)
            //                .ToList();
        }

        #endregion
    }
}
