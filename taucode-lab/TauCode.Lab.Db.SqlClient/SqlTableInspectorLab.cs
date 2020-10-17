﻿using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Model;
using TauCode.Extensions;

// todo: nice regions
namespace TauCode.Lab.Db.SqlClient
{
    public class SqlTableInspectorLab : DbTableInspectorBase
    {
        public SqlTableInspectorLab(SqlConnection connection, string schemaName, string tableName)
            : base(
                connection,
                schemaName ?? SqlToolsLab.DefaultSchemaName,
                tableName)
        {
        }

        protected SqlConnection SqlConnection => (SqlConnection)this.Connection;

        public override IDbUtilityFactory Factory => SqlUtilityFactoryLab.Instance;

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
    C.table_name = @p_tableName AND
    C.table_schema = @p_schema
ORDER BY
    C.ordinal_position
";

            command.AddParameterWithValue("p_tableName", this.TableName);
            command.AddParameterWithValue("p_schema", this.SchemaName);

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

            if (column.Type.Name.ToLowerInvariant().IsIn("text", "ntext"))
            {
                column.Type.Size = null;
            }
            else if (column.Type.Name.ToLowerInvariant().IsIn(
                "tinyint",
                "smallint",
                "int",
                "bigint",
                "smallmoney",
                "money",
                "float",
                "real"))
            {
                column.Type.Precision = null;
                column.Type.Scale = null;
            }

            return column;
        }

        protected override Dictionary<string, ColumnIdentityMold> GetIdentities()
        {
            var objectId = this.GetTableObjectId();

            using var command = this.Connection.CreateCommand();
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

        protected override bool NeedCheckSchemaExistence => true;

        protected override bool SchemaExists(string schemaName) =>
            this.SqlConnection.SchemaExists(this.SchemaName);

        protected override bool TableExists(string tableName) =>
            this.SqlConnection.TableExists(this.SchemaName, this.TableName);

        protected override PrimaryKeyMold GetPrimaryKeyImpl() =>
            this.SqlConnection.GetTablePrimaryKey(this.SchemaName, this.TableName);

        protected override IReadOnlyList<ForeignKeyMold> GetForeignKeysImpl()
            => this.SqlConnection.GetTableForeignKeys(this.SchemaName, this.TableName, true).ToList();

        protected override IReadOnlyList<IndexMold> GetIndexesImpl()
            => this.SqlConnection.GetTableIndexes(this.SchemaName, this.TableName).ToList();

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

            throw new ArgumentException(
                $"Could not parse value '{value}' of type '{value.GetType().FullName}' as boolean.");
        }

        private static int? GetDbValueAsInt(object dbValue)
        {
            if (dbValue == null)
            {
                return null;
            }

            return int.Parse(dbValue.ToString());
        }

        private int GetTableObjectId()
        {
            using var command = this.Connection.CreateCommand();
            command.CommandText =
                @"
SELECT
    T.object_id
FROM
    sys.tables T
INNER JOIN
    sys.schemas S
ON
    T.schema_id = S.schema_id
WHERE
    T.name = @p_tableName AND
    S.name = @p_schemaName
";
            command.AddParameterWithValue("p_tableName", this.TableName);
            command.AddParameterWithValue("p_schemaName", this.SchemaName);

            var objectResult = command.ExecuteScalar();

            if (objectResult == null)
            {
                // should not happen, we are checking table existence.
                throw DbTools.CreateInternalErrorException();
            }

            var objectId = (int)objectResult;
            return objectId;
        }
    }
}