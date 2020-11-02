using Npgsql;
using TauCode.Db;

namespace TauCode.Lab.Db.Npgsql
{
    public class NpgsqlSerializerLab : DbSerializerBase
    {
        public NpgsqlSerializerLab(NpgsqlConnection connection, string schemaName)
            : base(connection, schemaName ?? NpgsqlToolsLab.DefaultSchemaName)
        {
        }

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactoryLab.Instance;
    }
}
