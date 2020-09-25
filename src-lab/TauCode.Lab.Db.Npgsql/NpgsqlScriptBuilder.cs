using System.Text;
using TauCode.Db;
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

        protected override void WritePrimaryKeyConstraintScriptFragment(StringBuilder sb, PrimaryKeyMold primaryKey)
        {
            var decoratedConstraintName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Constraint,
                primaryKey.Name,
                this.CurrentOpeningIdentifierDelimiter);

            sb.Append($"CONSTRAINT {decoratedConstraintName} PRIMARY KEY(");

            for (var i = 0; i < primaryKey.Columns.Count; i++)
            {
                var indexColumn = primaryKey.Columns[i];
                var decoratedColumnName = this.Dialect.DecorateIdentifier(
                    DbIdentifierType.Column,
                    indexColumn.Name,
                    this.CurrentOpeningIdentifierDelimiter);

                sb.Append(decoratedColumnName);

                if (i < primaryKey.Columns.Count - 1)
                {
                    sb.Append(", ");
                }
            }

            sb.Append(")");
        }

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
