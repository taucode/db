using System.Text;
using TauCode.Db;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.Npgsql
{
    public class NpgsqlScriptBuilder : DbScriptBuilderBase
    {
        public NpgsqlScriptBuilder(string schemaName)
            : base(schemaName ?? NpgsqlTools.DefaultSchemaName)
        {
        }

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactory.Instance;

        protected override string BuildInsertScriptWithDefaultValues(TableMold table)
        {
            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                table.Name,
                this.CurrentOpeningIdentifierDelimiter);

            var sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            this.WriteSchemaPrefixIfNeeded(sb);
            sb.Append($"{decoratedTableName} DEFAULT VALUES");

            return sb.ToString();
        }
    }
}
