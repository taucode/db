using Newtonsoft.Json;
using System;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public abstract class DbMigratorBase : UtilityBase, IDbMigrator
    {
        protected DbMigratorBase(DbSerializerBase dbSerializer, Func<string> metadataJsonGetter, Func<string> dataJsonGetter)
            : base(dbSerializer.Connection, true, false)
        {
            this.DbSerializer = dbSerializer;
            this.MetadataJsonGetter = metadataJsonGetter ?? throw new ArgumentNullException(nameof(metadataJsonGetter));
            this.DataJsonGetter = dataJsonGetter ?? throw new ArgumentNullException(nameof(dataJsonGetter));
        }

        public IDbSerializer DbSerializer { get; }

        public Func<string> MetadataJsonGetter { get; }

        public Func<string> DataJsonGetter { get; }

        public void Migrate()
        {
            var scriptBuilder = this.DbSerializer.ScriptBuilder;
            var metadataJson = this.MetadataJsonGetter();
            var metadata = JsonConvert.DeserializeObject<DbMold>(metadataJson);

            var script = "WONT WORK TODO"; // todo! here!
            // var script = scriptBuilder.BuildCreateAllTablesScript(metadata); // todo! here!
            this.Connection.ExecuteCommentedScript(script);

            var dataJson = this.DataJsonGetter();
            this.DbSerializer.DeserializeDbData(dataJson);
        }
    }
}
