namespace TauCode.Db.SqlServer
{
    public class SqlServerScriptBuilder : ScriptBuilderBase
    {
        private const int MAX_SIZE_SURROGATE = -1;
        private const string MAX_SIZE = "max";

        public override IUtilityFactory Factory => SqlServerUtilityFactory.Instance;

        protected override string TransformNegativeTypeSize(int size)
        {
            if (size == MAX_SIZE_SURROGATE)
            {
                return MAX_SIZE;
            }

            return base.TransformNegativeTypeSize(size);
        }
    }
}
