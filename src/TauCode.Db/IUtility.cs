using System.Data;

namespace TauCode.Db
{
    public interface IUtility
    {
        IDbConnection Connection { get; }
        IUtilityFactory Factory { get; }
    }
}
