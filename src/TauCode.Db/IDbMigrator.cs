namespace TauCode.Db
{
    public interface IDbMigrator : IUtility
    {
        void Migrate();
    }
}
