using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Db.Utils.Crud;
using TauCode.Db.Utils.Crud.MySql;
using TauCode.Db.Utils.Dialects.MySql;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Utils.Inspection.MySql
{
    internal class MySqlTableInspector : TableInspectorBase
    {
        #region Constructor

        internal MySqlTableInspector(IDbConnection connection, string tableName)
            : base(MySqlDialect.Instance, connection, tableName)
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
    T.column_name                   ColumnName,
    T.data_type                     DataType,
    T.is_nullable                   IsNullable,
    T.character_maximum_length      MaxLen,
    T.numeric_precision             NumericPrecision,
    T.numeric_scale                 NumericScale,
    T.character_set_name            CharsetName,
    T.collation_name                CollationName
FROM
    information_schema.columns T
WHERE
    T.table_name = @p_tableName
    AND
    T.table_schema = @p_tableSchema
ORDER BY
    T.ordinal_position
";

                command.AddParameterWithValue("p_tableName", this.TableName);
                command.AddParameterWithValue("p_tableSchema", this.Connection.Database);

                return this.Cruder
                    .GetRows(command)
                    .Select(x => new ColumnInfo
                    {
                        Name = x.ColumnName,
                        TypeName = x.DataType,
                        IsNullable = this.ParseBoolean(x.IsNullable),
                        Size = UtilsHelper.GetDbValueAsInt(x.MaxLen),
                        Precision = UtilsHelper.GetDbValueAsInt(x.NumericPrecision),
                        Scale = UtilsHelper.GetDbValueAsInt(x.NumericScale),
                        AdditionalProperties = this.GetAdditionalProperties(x),
                    })
                    .ToList();
            }
        }

        private Dictionary<string, string> GetAdditionalProperties(dynamic row)
        {
            var dictionary = new Dictionary<string, string>();

            var characterSet = this.ParseString(row.CharSetName);
            if (characterSet != null)
            {
                dictionary.Add("CharacterSet", characterSet);
            }

            var collation = this.ParseString(row.CollationName);
            if (collation != null)
            {
                dictionary.Add("Collation", collation);
            }

            return dictionary;
        }

        protected override ColumnMold ColumnInfoToColumnMold(ColumnInfo columnInfo)
        {
            var columnMold = new ColumnMold
            {
                Name = columnInfo.Name,
                Type = this.Dialect.ResolveType(columnInfo.TypeName, columnInfo.Size, columnInfo.Precision, columnInfo.Scale),
                IsNullable = columnInfo.IsNullable,

                Properties = columnInfo.AdditionalProperties
                    .ToDictionary(x => x.Key, x => x.Value),
            };

            return columnMold;
        }

        protected override Dictionary<string, ColumnIdentityMold> GetIdentities()
        {
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText =
@"
SELECT
    T.column_name ColumnName
FROM
    information_schema.columns T
WHERE
    T.table_name = @p_tableName
    AND
    T.table_schema = @p_tableSchema
    AND
    T.extra LIKE @p_pattern
";
                command.AddParameterWithValue("p_tableName", this.TableName);
                command.AddParameterWithValue("p_tableSchema", this.Connection.Database);
                command.AddParameterWithValue("p_pattern", "%auto_increment%");

                return this.Cruder
                    .GetRows(command)
                    .ToDictionary(
                        x => (string)x.ColumnName,
                        x => new ColumnIdentityMold
                        {
                            Seed = (1).ToString(),
                            Increment = (1).ToString(),
                        });
            }
        }

        protected override ICruder CreateCruder()
        {
            return new MySqlCruder();
        }

        public override PrimaryKeyMold GetPrimaryKeyMold()
        {
            var pkIndex = this.GetIndexMolds().Single(x => x.Name == "PRIMARY");
            return new PrimaryKeyMold
            {
                Name = pkIndex.Name,
                ColumnNames = pkIndex.ColumnNames.ToList(),
            };
        }

        public override List<ForeignKeyMold> GetForeignKeyMolds()
        {
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText =
@"
SELECT
    KCU.constraint_name                 ConstraintName,
    KCU.referenced_table_name           ReferencedTableName,
    KCU.column_name                     ColumnName,
    KCU.referenced_column_name          ReferencedColumnName,
    KCU.ordinal_position                OrdinalPosition,
    KCU.position_in_unique_constraint   PositionInUniqueConstraint
FROM
    information_schema.key_column_usage KCU
WHERE
    KCU.table_schema = @p_schemaName
    AND
    KCU.table_name = @p_tableName
    AND
    KCU.referenced_table_name IS NOT NULL
";

                command.AddParameterWithValue("p_schemaName", this.Connection.Database);
                command.AddParameterWithValue("p_tableName", this.TableName);

                var foreignKeys = this.Cruder
                    .GetRows(command)
                    .GroupBy(x => (string)x.ConstraintName)
                    .Where(g => g.Key != "PRIMARY")
                    .Select(g => new ForeignKeyMold
                    {
                        Name = g.Key,
                        ReferencedTableName = (string)g.First().ReferencedTableName,
                        ColumnNames = g
                            .OrderBy(x => (uint)x.OrdinalPosition)
                            .Select(x => (string)x.ColumnName)
                            .ToList(),
                        ReferencedColumnNames = g
                            .OrderBy(x => (uint)x.PositionInUniqueConstraint)
                            .Select(x => (string)x.ReferencedColumnName)
                            .ToList(),
                    })
                    .ToList();

                return foreignKeys;
            }
        }

        public override List<IndexMold> GetIndexMolds()
        {
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText = $@"SHOW INDEX FROM `{this.TableName}`";

                var indexMolds = this.Cruder
                    .GetRows(command)
                    .GroupBy(x => (string)x.key_name)
                    .Select(g => new IndexMold
                    {
                        Name = (string)g.Key,
                        IsUnique = (int)g.First().non_unique == 0,
                        ColumnNames = g
                            .OrderBy(x => (uint)x.seq_in_index)
                            .Select(x => (string)x.column_name)
                            .ToList(),
                    })
                    .ToList();

                return indexMolds;
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

        private int? ParseNullableInt(object value)
        {
            if (value == DBNull.Value)
            {
                return null;
            }

            else if (value.GetType().IsIn(
                typeof(int),
                typeof(uint),
                typeof(long),
                typeof(ulong)))
            {
                var n = int.Parse(value.ToString());
                return n;
            }

            throw new ArgumentException($"Could not parse value '{value}' of type '{value.GetType().FullName}' as nullable int.");
        }

        private string ParseString(object value)
        {
            if (value == DBNull.Value)
            {
                return null;
            }

            if (value is string s)
            {
                return s;
            }

            throw new ArgumentException($"Could not parse value '{value}' of type '{value.GetType().FullName}' as string.");
        }

        #endregion
    }
}