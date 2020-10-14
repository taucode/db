using System.Collections.Generic;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient
{
    public class SqlInspectorLab : DbInspectorBase
    {
        public SqlInspectorLab(IDbConnection connection, string schemaName)
            : base(connection, schemaName)
        {
        }

        public override IDbUtilityFactory Factory => SqlUtilityFactoryLab.Instance;

        protected override IReadOnlyList<string> GetTableNamesImpl(string schemaName)
        {
            throw new System.NotImplementedException();
        }

        protected override HashSet<string> GetSystemSchemata()
        {
            throw new System.NotImplementedException();
        }
    }
}
