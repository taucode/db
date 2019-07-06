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

                var columnInfos = this.Cruder
                    .GetRows(command)
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
    KCU.column_name ColumnName
FROM
    information_schema.key_column_usage KCU
WHERE
    KCU.constraint_name = @p_constraintName
    AND
    KCU.table_name = @p_tableName
ORDER BY
    KCU.ordinal_position
";
                command.AddParameterWithValue("p_constraintName", constraintName);
                command.AddParameterWithValue("p_tableName", this.TableName);

                var columnNames = this.Cruder
                    .GetRows(command)
                    .Select(x => (string)x.ColumnName)
                    .ToList();

                var primaryKeyMold = new PrimaryKeyMold
                {
                    Name = constraintName,
                    ColumnNames = columnNames,
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
