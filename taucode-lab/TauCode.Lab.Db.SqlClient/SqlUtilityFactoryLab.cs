using Microsoft.Data.SqlClient;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlUtilityFactoryLab : IDbUtilityFactory
    {
        public static SqlUtilityFactoryLab Instance { get; } = new SqlUtilityFactoryLab();

        private SqlUtilityFactoryLab()
        {
        }

        public IDbDialect GetDialect() => SqlDialectLab.Instance;

        public IDbScriptBuilder CreateScriptBuilder(string schema) => new SqlScriptBuilderLab(schema);
        public IDbConnection CreateConnection() => new SqlConnection();

        public IDbInspector CreateInspector(IDbConnection connection, string schema) => new SqlInspectorLab(connection, schema);

        public IDbTableInspector CreateTableInspector(IDbConnection connection, string schema, string tableName) =>
            new SqlTableInspectorLab(connection, schema, tableName);

        public IDbCruder CreateCruder(IDbConnection connection, string schema) => new SqlCruderLab(connection, schema);

        public IDbSerializer CreateSerializer(IDbConnection connection, string schema) => new SqlSerializerLab(connection, schema);
    }
}
