using System.Data;

namespace TauCode.Db
{
    public interface IDbParameterInfo
    {
        string ParameterName { get; }
        DbType DbType { get; }
        int? Size { get; }
        int? Precision { get; }
        int? Scale { get; }
    }
}
