﻿using TauCode.Db;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.Npgsql
{
    public class NpgsqlScriptBuilder : DbScriptBuilderBase
    {
        //private const int MAX_SIZE_SURROGATE = -1;
        //private const string MAX_SIZE = "max";

        public NpgsqlScriptBuilder(string schema)
            : base(schema)
        {
        }

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactory.Instance;

        //protected override string TransformNegativeTypeSize(int size)
        //{
        //    if (size == MAX_SIZE_SURROGATE)
        //    {
        //        return MAX_SIZE;
        //    }

        //    return base.TransformNegativeTypeSize(size);
        //}

        protected override string BuildInsertScriptWithDefaultValues(TableMold table)
        {
            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                table.Name,
                this.CurrentOpeningIdentifierDelimiter);

            var result = $"INSERT INTO {decoratedTableName} DEFAULT VALUES";
            return result;
        }
    }
}