using System;

namespace TauCode.Db.SqlServer
{
    public class SqlServerMigrator : DbMigratorBase
    {
        public SqlServerMigrator(
            SqlServerSerializer serializer,
            Func<string> metadataJsonGetter,
            Func<string> dataJsonGetter)
            : base(serializer, metadataJsonGetter, dataJsonGetter)
        {
        }

        public override IUtilityFactory Factory => SqlServerUtilityFactory.Instance;
    }
}
