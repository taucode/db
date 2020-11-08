﻿using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;
using TauCode.Db;
using TauCode.Db.DbValueConverters;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.Npgsql
{
    public class NpgsqlCruderLab : DbCruderBase
    {
        public NpgsqlCruderLab(NpgsqlConnection connection, string schemaName)
            : base(connection, schemaName ?? NpgsqlToolsLab.DefaultSchemaName)
        {
        }

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactoryLab.Instance;
        protected override IDbValueConverter CreateDbValueConverter(ColumnMold column)
        {
            switch (column.Type.Name)
            {
                case "uuid":
                    return new GuidValueConverter();

                case "boolean":
                    return new BooleanValueConverter();

                case "smallint":
                    return new Int16ValueConverter();

                case "integer":
                    return new Int32ValueConverter();

                case "bigint":
                    return new Int64ValueConverter();

                case "numeric":
                case "money":
                    return new DecimalValueConverter();

                case "double precision":
                    return new DoubleValueConverter();

                case "real":
                    return new SingleValueConverter();

                case "timestamp without time zone":
                    return new DateTimeValueConverter();

                case "timestamp with time zone":
                    return new DateTimeOffsetValueConverter();

                case "time without time zone":
                    return new TimeSpanValueConverter();

                case "character":
                case "character varying":
                case "text":
                    return new StringValueConverter();

                case "bytea":
                    return new ByteArrayValueConverter();

                default:
                    throw new NotImplementedException();
            }
        }

        protected override IDbDataParameter CreateParameter(string tableName, ColumnMold column)
        {
            const string parameterName = "parameter_name_placeholder";

            switch (column.Type.Name)
            {
                case "uuid":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Uuid);

                case "boolean":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Boolean);

                case "smallint":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Smallint);

                case "integer":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Integer);

                case "bigint":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Bigint);

                case "numeric":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Numeric);

                case "money":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Money);

                case "real":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Real);

                case "double precision":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Double);

                case "timestamp without time zone":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Timestamp);

                case "timestamp with time zone":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.TimestampTz);

                case "time without time zone":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Time);

                case "character":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Char);

                case "character varying":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Varchar);

                case "text":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Text);

                case "bytea":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Bytea);

                default:
                    throw new NotImplementedException();
            }
        }

        protected override bool NeedCheckSchemaExistence => false; // Table inspector will do the job.

        protected override bool SchemaExists(string schemaName)
        {
            throw new InvalidOperationException("Internal error: should not be called.");
        }
    }
}
