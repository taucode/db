using System.Data;

namespace TauCode.Db
{
    public abstract class DbMigratorBase : UtilityBase, IDbMigrator
    {
        public DbMigratorBase(IDbConnection connection)
            : base(connection, true, false)
        {
        }

        public abstract void Migrate();
    }
}
