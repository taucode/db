using System.Data;

namespace TauCode.Db
{
    public class ParameterInfoImpl : IParameterInfo
    {
        public ParameterInfoImpl(
            string parameterName,
            DbType dbType,
            int? size,
            int? precision,
            int? scale)
        {
            this.ParameterName = parameterName;
            this.DbType = dbType;
            this.Size = size;
            this.Precision = precision;
            this.Scale = scale;
        }

        public string ParameterName { get; }
        public DbType DbType { get; }
        public int? Size { get; }
        public int? Precision { get; }
        public int? Scale { get; }
    }
}
