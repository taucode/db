using NUnit.Framework;
using System.Data;
using System.Data.SQLite;

namespace TauCode.Db.Tests
{
    [TestFixture]
    public abstract class SQLiteTestBase : TestBase
    {
        protected override IDbConnection CreateConnection()
        {
            SQLiteConnection.CreateFile(this.TempDbFilePath);
            var connection = new SQLiteConnection(this.ConnectionString);
            return connection;
        }

        protected override string CreateConnectionString()
        {
            var connectionString = $"Data Source={this.TempDbFilePath};Version=3;";
            return connectionString;
        }

        protected override string ResourceName => "sqlite-create-db.sql";
    }
}
