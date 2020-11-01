using System.Text;
using TauCode.Db;
using TauCode.Db.Model;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlScriptBuilderLab : DbScriptBuilderBase
    {
        private const int MAX_SIZE_SURROGATE = -1;
        private const string MAX_SIZE = "max";

        public SqlScriptBuilderLab(string schema)
            : base(schema ?? SqlToolsLab.DefaultSchemaName)
        {

        }

        public override IDbUtilityFactory Factory => SqlUtilityFactoryLab.Instance;

        protected override string TransformNegativeTypeSize(int size)
        {
            if (size == MAX_SIZE_SURROGATE)
            {
                return MAX_SIZE;
            }

            return base.TransformNegativeTypeSize(size);
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
            sb.Append($"{decoratedTableName} DEFAULT VALUES");

            return sb.ToString();
        }
    }
}
