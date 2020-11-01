namespace TauCode.Db
{
    public interface IDbMigrator : IDbUtility
    {
        string SchemaName { get; }
        void Migrate();
    }
}
