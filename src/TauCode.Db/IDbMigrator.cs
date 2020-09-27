namespace TauCode.Db
{
    public interface IDbMigrator : IDbUtility
    {
        string Schema { get; }
        void Migrate();
    }
}
