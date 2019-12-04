using TauCode.Db.Model;

namespace TauCode.Db
{
    public interface IDbConverter : IUtility
    {
        DbMold ConvertDb(string targetDbProviderName);
    }
}
