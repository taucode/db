using Newtonsoft.Json;
using System;
using System.Data;
using TauCode.Db.Data;
using TauCode.Db.Exceptions;
using TauCode.Db.Extensions;
using TauCode.Db.Model;
using TauCode.Db.Schema;

// todo clean up
namespace TauCode.Db
{
    public abstract class DbJsonMigratorBase : DbUtilityBase, IDbMigrator
    {
        #region Fields

        private IDbSerializer _serializer;
        private IDbSchemaExplorer _schemaExplorer;

        #endregion

        #region Constructor

        protected DbJsonMigratorBase(
            IDbConnection connection,
            string schemaName,
            Func<string> metadataJsonGetter,
            Func<string> dataJsonGetter,
            Func<string, bool> tableNamePredicate = null,
            Func<TableMold, DynamicRow, DynamicRow> rowTransformer = null)
            : base(connection, true, false)
        {
            this.SchemaName = schemaName;
            this.MetadataJsonGetter = metadataJsonGetter ?? throw new ArgumentNullException(nameof(metadataJsonGetter));
            this.DataJsonGetter = dataJsonGetter ?? throw new ArgumentNullException(nameof(dataJsonGetter));
            this.TableNamePredicate = tableNamePredicate;
            this.RowTransformer = rowTransformer;
        }

        #endregion

        #region Protected

        protected abstract IDbSchemaExplorer CreateSchemaExplorer(IDbConnection connection);

        protected IDbSchemaExplorer SchemaExplorer => _schemaExplorer ??= this.CreateSchemaExplorer(this.Connection);

        protected virtual void CheckSchemaIfNeeded()
        {
            this.SchemaExplorer.CheckSchema(this.SchemaName);
        }

        #endregion

        #region Abstract & Virtual

        protected virtual IDbSerializer CreateSerializer()
        {
            var serializer = this.Factory.CreateSerializer(this.Connection, this.SchemaName);
            return serializer;
        }

        #endregion

        #region Public

        public Func<string, bool> TableNamePredicate { get; }
        public Func<TableMold, DynamicRow, DynamicRow> RowTransformer { get; }
        public IDbSerializer Serializer => _serializer ??= this.CreateSerializer();
        public Func<string> MetadataJsonGetter { get; }
        public Func<string> DataJsonGetter { get; }

        #endregion

        #region IDbMigrator Members

        public string SchemaName { get; }

        public virtual void Migrate()
        {
            this.CheckSchemaIfNeeded();

            // migrate metadata
            var metadataJson = this.MetadataJsonGetter();
            if (metadataJson == null)
            {
                throw new TauDbException($"'{nameof(MetadataJsonGetter)}' returned null.");
            }

            var metadata = JsonConvert.DeserializeObject<DbMold>(metadataJson);
            var dialect = this.Factory.GetDialect();
            var predicate = this.TableNamePredicate ?? (x => true);

            using (var command = this.Connection.CreateCommand())
            {
                foreach (var table in metadata.Tables)
                {
                    if (!predicate(table.Name))
                    {
                        continue;
                    }

                    // create table itself
                    var script = this.Serializer.Cruder.ScriptBuilder.BuildCreateTableScript(
                        table,
                        true);
                    command.CommandText = script;
                    command.ExecuteNonQuery();

                    var indexes = dialect.GetCreatableIndexes(table);

                    // create indexes
                    foreach (var index in indexes)
                    {
                        script = this.Serializer.Cruder.ScriptBuilder.BuildCreateIndexScript(index);
                        command.CommandText = script;
                        command.ExecuteNonQuery();
                    }
                }
            }

            // migrate data
            var dataJson = this.DataJsonGetter();

            if (dataJson == null)
            {
                throw new TauDbException($"'{nameof(DataJsonGetter)}' returned null.");
            }

            this.Serializer.DeserializeDbData(dataJson, this.TableNamePredicate, this.RowTransformer);
        }

        #endregion
    }
}
