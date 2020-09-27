using Npgsql;
using NpgsqlTypes;
using System;
using System.Data;
using TauCode.Db;
using TauCode.Db.DbValueConverters;
using TauCode.Db.Model;
using TauCode.Lab.Db.Npgsql.DbValueConverters;

namespace TauCode.Lab.Db.Npgsql
{
    public class NpgsqlCruder : DbCruderBase
    {
        public NpgsqlCruder(IDbConnection connection, string schema)
            : base(connection, schema)
        {
        }

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactory.Instance;

        protected override IDbValueConverter CreateDbValueConverter(ColumnMold column)
        {
            var typeName = column.Type.Name.ToLowerInvariant();
            switch (typeName)
            {
                case "uuid":
                    return new GuidValueConverter();

                case "character":
                case "character varying":
                case "text":
                    return new StringValueConverter();

                case "integer":
                    return new Int32ValueConverter();

                case "date":
                    return new DateTimeValueConverter();

                case "bit":
                    return new BooleanValueConverter();

                case "float":
                    return new DoubleValueConverter();

                case "real":
                    return new SingleValueConverter();

                case "money":
                    return new NpgsqlMoneyConverter();

                case "decimal":
                case "numeric":
                    return new DecimalValueConverter();

                case "double precision":
                    return new DoubleValueConverter();

                case "smallint":
                    return new Int16ValueConverter();

                case "bigint":
                    return new Int64ValueConverter();

                case "timestamp without time zone":
                    return new DateTimeValueConverter();

                case "boolean":
                    return new BooleanValueConverter();

                case "bytea":
                    return new ByteArrayValueConverter();

                default:
                    throw new NotImplementedException();
            }
        }

        protected override IDbDataParameter CreateParameter(string tableName, ColumnMold column, string parameterName)
        {
            switch (column.Type.Name)
            {
                case "uuid":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Uuid);

                case "character":
                    return new NpgsqlParameter(
                        parameterName,
                        NpgsqlDbType.Char,
                        column.Type.Size ?? throw new NotImplementedException());

                case "character varying":
                    return new NpgsqlParameter(
                        parameterName,
                        NpgsqlDbType.Varchar,
                        column.Type.Size ?? throw new NotImplementedException());

                case "text":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Text, -1);

                case "timestamp without time zone":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Timestamp);

                case "boolean":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Boolean);

                case "bytea":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Bytea, -1);

                case "smallint":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Smallint);

                case "integer":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Integer);

                case "bigint":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Bigint);

                case "double precision":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Double);

                case "real":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Real);

                case "money":
                    return new NpgsqlParameter(parameterName, NpgsqlDbType.Money);

                case "numeric":
                    var parameter = new NpgsqlParameter(parameterName, NpgsqlDbType.Numeric);
                    parameter.Precision = (byte)(column.Type.Precision ?? 0);
                    parameter.Scale = (byte)(column.Type.Scale ?? 0);
                    return parameter;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
