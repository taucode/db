using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Schema;

// todo clean & regions.
namespace TauCode.Lab.Db.SqlClient
{
    public class SqlInspectorLab : DbInspectorBase
    {
        public SqlInspectorLab(SqlConnection connection, string schemaName)
            : base(connection, schemaName ?? SqlToolsLab.DefaultSchemaName)
        {
            this.SchemaExplorer = new SqlSchemaExplorer(this.SqlConnection);
        }

        protected SqlConnection SqlConnection => (SqlConnection)this.Connection;

        protected IDbSchemaExplorer SchemaExplorer { get; }

        public override IDbUtilityFactory Factory => SqlUtilityFactoryLab.Instance;

        protected override IReadOnlyList<string> GetTableNamesImpl(string schemaName) =>
            this.SchemaExplorer.GetTableNames(schemaName);
        //this.SqlConnection.GetTableNames(this.SchemaName, null);

        protected override HashSet<string> GetSystemSchemata()
            //=> SqlToolsLab.SystemSchemata;
            => this.SchemaExplorer.GetSystemSchemata().ToHashSet(); // todo ugly!

        protected override bool NeedCheckSchemaExistence => throw new NotImplementedException();

        protected override bool SchemaExists(string schemaName)
        {
            throw new System.NotImplementedException();
        }

        public override IReadOnlyList<string> GetTableNames()
        {
            return this.SchemaExplorer.GetTableNames(this.SchemaName);
        }

        //protected override bool NeedCheckSchemaExistence => true;

        //protected override bool SchemaExists(string schemaName) => this.SqlConnection.SchemaExists(schemaName);
    }
}
