using System;
using System.Text;
using TauCode.Db.Model;

namespace TauCode.Db.MySql
{
    public class MySqlScriptBuilder : DbScriptBuilderBase
    {
        public MySqlScriptBuilder(string schemaName)
            : base(schemaName)
        {
            if (schemaName == null)
            {
                throw new ArgumentNullException(nameof(schemaName)); // todo: ut this
            }
        }

        public override IDbUtilityFactory Factory => MySqlUtilityFactory.Instance;

        protected override void WriteIdentityScriptFragment(StringBuilder sb, ColumnIdentityMold identity)
        {
            sb.Append("AUTO_INCREMENT");
        }

        protected override void WriteTypeDefinitionScriptFragment(StringBuilder sb, DbTypeMold type)
        {
            base.WriteTypeDefinitionScriptFragment(sb, type);

            if (type.Properties.ContainsKey("character_set_name"))
            {
                sb.Append(" CHARACTER SET ");
                sb.Append(type.Properties["character_set_name"]);
            }

            if (type.Properties.ContainsKey("collation_name"))
            {
                sb.Append(" COLLATE ");
                sb.Append(type.Properties["collation_name"]);
            }

            if (type.Properties.ContainsKey("unsigned"))
            {
                sb.Append(" unsigned");
            }
        }

        protected override string BuildInsertScriptWithDefaultValues(TableMold table)
        {
            var decoratedTableName = this.Dialect.DecorateIdentifier(
                DbIdentifierType.Table,
                table.Name,
                this.CurrentOpeningIdentifierDelimiter);

            var sb = new StringBuilder();
            sb.Append("INSERT INTO ");
            this.WriteSchemaPrefixIfNeeded(sb);
            sb.Append($"{decoratedTableName} VALUES()");

            return sb.ToString();
        }
    }
}
