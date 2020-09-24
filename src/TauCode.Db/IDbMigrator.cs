namespace TauCode.Db
{
    public interface IDbMigrator : IDbUtility
    {
        void Migrate();
    }
}
