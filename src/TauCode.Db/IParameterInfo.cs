using System.Data;

namespace TauCode.Db
{
    public interface IParameterInfo
    {
        string ParameterName { get; }
        DbType DbType { get; }
        int? Size { get; }
        int? Precision { get; }
        int? Scale { get; }

    }
}
