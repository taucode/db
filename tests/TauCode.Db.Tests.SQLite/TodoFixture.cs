using NUnit.Framework;
using System.Data.SQLite;
using TauCode.Extensions;

namespace TauCode.Db.Tests.SQLite
{
    [TestFixture]
    public class TodoFixture
    {
        [Test]
        public void TodoTest()
        {
            var tuple = DbUtils.CreateSQLiteConnectionString();
            var fileName = tuple.Item1;
            var connectionString = tuple.Item2;

            var factory = DbUtils.GetUtilityFactory(DbProviderNames.SQLite);

            using (var connection = (SQLiteConnection)factory.CreateConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                connection.BoostSQLiteInsertions();

                var sql = @"
CREATE TABLE currency(
    [id] UNIQUEIDENTIFIER PRIMARY KEY,
    [code] TEXT NOT NULL,
    [name] TEXT NOT NULL)
";
                connection.ExecuteSingleSql(sql);

                sql = @"
CREATE TABLE VersionInfo(
    [Version] INTEGER NOT NULL,
    [AppliedOn] DATETIME NOT NULL,
    [Description] TEXT NOT NULL
)
";
                connection.ExecuteSingleSql(sql);

                var json = this.GetType().Assembly.GetResourceText(".TodoData.json", true);

                var dbSerializer = factory.CreateDbSerializer(connection);
                dbSerializer.DeserializeDbData(json);
            }
        }
    }
}
