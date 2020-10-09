using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace TauCode.Db
{
    public abstract class DbInspectorBase : DbUtilityBase, IDbInspector
    {
        #region Constructor

        protected DbInspectorBase(IDbConnection connection, string schema)
            : base(connection, true, false)
        {
            this.Schema = schema;
        }

        #endregion

        #region Abstract & Virtual

        protected abstract IReadOnlyList<string> GetTableNamesImpl(string schema);

        protected abstract HashSet<string> GetSystemSchemata();

        #endregion

        #region IDbInspector Members

        public string Schema { get; }

        public virtual IReadOnlyList<string> GetSchemata()
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
                .Except(systemSchemata)
                .ToList();

            return schemata;
        }

        public IReadOnlyList<string> GetTableNames()
        {
            var tableNames = this.GetTableNamesImpl(this.Schema);

            return tableNames;
        }

        #endregion
    }
}
