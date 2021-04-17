using Npgsql;

namespace TauCode.Db.Npgsql
{
    public class NpgsqlSerializer : DbSerializerBase
    {
        public NpgsqlSerializer(NpgsqlConnection connection, string schemaName)
            : base(connection, schemaName ?? NpgsqlTools.DefaultSchemaName)
        {
        }

        public override IDbUtilityFactory Factory => NpgsqlUtilityFactory.Instance;
    }
}
