using System.Data;

namespace TauCode.Db
{
    public interface IDbUtility
    {
        IDbConnection Connection { get; }
        IDbUtilityFactory Factory { get; }
    }
}
