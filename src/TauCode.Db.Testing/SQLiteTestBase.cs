using System.Data.SQLite;
using System.IO;
using TauCode.Db.SQLite;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Testing
{
    public abstract class SQLiteTestBase : DbTestBase
    {
        protected string DbFilePath { get; set; }

        protected override string GetConnectionString()
        {
            this.DbFilePath = FileExtensions.CreateTempFilePath("zunit", ".sqlite");
            SQLiteConnection.CreateFile(this.DbFilePath);
            var connectionString = $"Data Source={this.DbFilePath};Version=3;";
            return connectionString;
        }

        protected override void OneTimeSetUpImpl()
        {
            base.OneTimeSetUpImpl();

            // tune connection for performance
            // see here: https://stackoverflow.com/questions/3852068/sqlite-insert-very-slow
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText = "PRAGMA journal_mode = WAL";
                command.ExecuteNonQuery();

                command.CommandText = "PRAGMA synchronous = NORMAL";
                command.ExecuteNonQuery();
            }

            this.DbMigrator = this.CreateDbMigrator();
        }

        protected override void OneTimeTearDownImpl()
        {
            base.OneTimeTearDownImpl();
            try
            {
                File.Delete(this.DbFilePath);
            }
            catch
            {
                // dismiss
            }
        }

        protected override IDbMigrator CreateDbMigrator()
        {
            var migrator = new SQLiteJsonMigrator(
                this.Connection,
                this.GetMetadataJson,
                this.GetDataJson);

            return migrator;
        }

        protected abstract string GetMetadataJson();
        protected abstract string GetDataJson();
    }
}
