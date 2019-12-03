using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Db.Utils.Dialects;

namespace TauCode.Db.Utils.Inspection
{
    public abstract class SqlServerTableInspectorBase : TableInspectorBase
    {
        #region Constructor

        protected SqlServerTableInspectorBase(IDialect dialect, IDbConnection connection, string tableName)
            : base(dialect, connection, tableName)
        {
        }

        #endregion

        #region Overridden

        protected override List<ColumnInfo> GetColumnInfos()
        {
            using (var command = this.Connection.CreateCommand())
            {
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

                var columnInfos = UtilsHelper
                    .GetCommandRows(command)
                    .Select(x => new ColumnInfo
                    {
                        Name = x.ColumnName,
                        TypeName = x.DataType,
                        IsNullable = this.ParseBoolean(x.IsNullable),
                        Size = UtilsHelper.GetDbValueAsInt(x.MaxLen),
                        Precision = UtilsHelper.GetDbValueAsInt(x.NumericPrecision),
                        Scale = UtilsHelper.GetDbValueAsInt(x.NumericScale),
                    })
                    .ToList();

                return columnInfos;
            }
        }

        protected override ColumnMold ColumnInfoToColumnMold(ColumnInfo columnInfo)
        {
            var column = new ColumnMold
            {
                Name = columnInfo.Name,
                Type = this.Dialect.ResolveType(
                    columnInfo.TypeName,
                    columnInfo.Size,
                    columnInfo.Precision,
                    columnInfo.Scale),
                IsNullable = columnInfo.IsNullable,
            };

            return column;
        }

        public override PrimaryKeyMold GetPrimaryKeyMold()
        {
            using (var command = this.Connection.CreateCommand())
            {
                // get PK name
                command.CommandText =
@"
SELECT
    TC.constraint_name
FROM
    information_schema.table_constraints TC
WHERE
    TC.table_name = @p_tableName
    AND
    TC.constraint_type = 'PRIMARY KEY'";

                var parameter = command.CreateParameter();
                parameter.ParameterName = "p_tableName";
                parameter.Value = this.TableName;
                command.Parameters.Add(parameter);

                var constraintName = (string)command.ExecuteScalar();

                if (constraintName == null)
                {
                    return null;
                }

                // get PK columns
                command.Parameters.Clear();
                command.CommandText =
@"

SELECT
    I.[index_id]            IndexId,
    I.[name]                IndexName,
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
    I.[name] = @p_constraintName AND
    T.[name] = @p_tableName
ORDER BY
    IC.[key_ordinal]
";
                command.AddParameterWithValue("p_constraintName", constraintName);
                command.AddParameterWithValue("p_tableName", this.TableName);


                var columns = UtilsHelper
                    .GetCommandRows(command)
                    .Select(x => new IndexColumnMold
                    {
                        Name = (string)x.ColumnName,
                        SortDirection = (bool)x.IsDescendingKey ? SortDirection.Descending : SortDirection.Ascending,
                    })
                    .ToList();

                var primaryKeyMold = new PrimaryKeyMold
                {
                    Name = constraintName,
                    Columns = columns,
                };

                return primaryKeyMold;
            }
        }

        #endregion

        #region Private

        private bool ParseBoolean(object value)
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

        #endregion
    }
}
