﻿using System.Data;
using Microsoft.Data.SqlClient;
using TauCode.Db.Schema;
using TauCode.Db.SqlClient.Schema;

namespace TauCode.Db.SqlClient
{
    public class SqlTableInspector : DbTableInspectorBase
    {
        public SqlTableInspector(SqlConnection connection, string schemaName, string tableName)
            : base(
                connection,
                schemaName ?? SqlTools.DefaultSchemaName,
                tableName)
        {
        }

        public override IDbUtilityFactory Factory => SqlUtilityFactory.Instance;

        protected override IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection) =>
            new SqlSchemaExplorer((SqlConnection) this.Connection);
    }
}
