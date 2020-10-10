using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TauCode.Db
{
    public abstract class DbInspectorBase : DbUtilityBase, IDbInspector
    {
        #region Constructor

        protected DbInspectorBase(IDbConnection connection, string schemaName)
            : base(connection, true, false)
        {
            this.SchemaName = schemaName;
        }

        #endregion

        #region Abstract & Virtual

        protected abstract IReadOnlyList<string> GetTableNamesImpl(string schemaName);

        protected abstract HashSet<string> GetSystemSchemata();

        #endregion

        #region IDbInspector Members

        public string SchemaName { get; }

        public virtual IReadOnlyList<string> GetSchemaNames()
        {
            var systemSchemata = this.GetSystemSchemata();

            using var command = this.Connection.CreateCommand();
            command.CommandText =
                @"
SELECT
    schema_name SchemaName
FROM
    information_schema.schemata
";

            var schemata = DbTools
                .GetCommandRows(command)
                .Select(x => (string)x.SchemaName)
                .Except(systemSchemata, StringComparer.InvariantCultureIgnoreCase)
                .ToList();

            return schemata;
        }

        public IReadOnlyList<string> GetTableNames()
        {
            var tableNames = this.GetTableNamesImpl(this.SchemaName);

            return tableNames;
        }

        #endregion
    }
}
