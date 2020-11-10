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

        #region Private

        private void CheckSchemaIfNeeded()
        {
            if (this.NeedCheckSchemaExistence)
            {
                if (!this.SchemaExists(this.SchemaName))
                {
                    throw DbTools.CreateSchemaDoesNotExistException(this.SchemaName);
                }
            }
        }

        #endregion

        #region Abstract & Virtual

        protected abstract IReadOnlyList<string> GetTableNamesImpl(string schemaName);

        protected abstract HashSet<string> GetSystemSchemata();

        protected abstract bool NeedCheckSchemaExistence { get; }

        protected abstract bool SchemaExists(string schemaName);

        #endregion

        #region IDbInspector Members

        public string SchemaName { get; }

        // todo: move to metadata explorer (ex-schema-explorer)
        public virtual IReadOnlyList<string> GetSchemaNames()
        {
            var systemSchemata = this.GetSystemSchemata();

            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    S.schema_name SchemaName
FROM
    information_schema.schemata S
ORDER BY
    S.schema_name
";

            var schemata = command
                .GetCommandRows()
                .Select(x => (string)x.SchemaName)
                .Except(systemSchemata)
                .ToList();

            return schemata;
        }

        public virtual IReadOnlyList<string> GetTableNames()
        {
            this.CheckSchemaIfNeeded();

            var tableNames = this.GetTableNamesImpl(this.SchemaName);

            return tableNames;
        }

        #endregion
    }
}
