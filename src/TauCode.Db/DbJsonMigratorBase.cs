using Newtonsoft.Json;
using System;
using System.Data;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public abstract class DbJsonMigratorBase : DbUtilityBase, IDbMigrator
    {
        #region Fields

        private IDbSerializer _dbSerializer;

        #endregion

        #region Constructor

        protected DbJsonMigratorBase(
            IDbConnection connection,
            string schema,
            Func<string> metadataJsonGetter,
            Func<string> dataJsonGetter)
            : base(connection, true, false)
        {
            this.Schema = schema;
            this.MetadataJsonGetter = metadataJsonGetter ?? throw new ArgumentNullException(nameof(metadataJsonGetter));
            this.DataJsonGetter = dataJsonGetter ?? throw new ArgumentNullException(nameof(dataJsonGetter));
        }

        #endregion

        #region Protected

        protected virtual IDbSerializer DbSerializer => _dbSerializer ??= this.Factory.CreateDbSerializer(this.Connection, this.Schema);

        #endregion

        #region Public

        public Func<string> MetadataJsonGetter { get; }
        public Func<string> DataJsonGetter { get; }
        public string Schema { get; }

        #endregion

        #region IDbMigrator Members

        public virtual void Migrate()
        {
            // migrate metadata
            var metadataJson = this.MetadataJsonGetter();
            var metadata = JsonConvert.DeserializeObject<DbMold>(metadataJson);

            using (var command = this.Connection.CreateCommand())
            {
                foreach (var table in metadata.Tables)
                {
                    // create table itself
                    var script = this.DbSerializer.Cruder.ScriptBuilder.BuildCreateTableScript(
                        table,
                        true);
                    command.CommandText = script;
                    command.ExecuteNonQuery();

                    // create indexes
                    foreach (var index in table.Indexes)
                    {
                        script = this.DbSerializer.Cruder.ScriptBuilder.BuildCreateIndexScript(index);
                        command.CommandText = script;
                        command.ExecuteNonQuery();
                    }
                }
            }

            // migrate data
            var dataJson = this.DataJsonGetter();
            this.DbSerializer.DeserializeDbData(dataJson);
        }

        #endregion
    }
}
