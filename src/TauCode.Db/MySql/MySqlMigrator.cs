using System;

namespace TauCode.Db.MySql
{
    public class MySqlMigrator : DbMigratorBase
    {
        public MySqlMigrator(
            MySqlSerializer serializer,
            Func<string> metadataJsonGetter,
            Func<string> dataJsonGetter)
            : base(serializer, metadataJsonGetter, dataJsonGetter)
        {
        }

        public override IUtilityFactory Factory => MySqlUtilityFactory.Instance;
    }
}
