using NUnit.Framework;
using System.Data;
using System.IO;
using TauCode.Db.Migrations;
using TauCode.Db.Tests.Data;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Serialization;
using TauCode.Db.Utils.Serialization.SQLite;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Tests.Utils.Serialization.SQLite
{
    [TestFixture]
    public class SQLiteSerializerTests
    {
        //private IDbInspector _dbInspector;
        private IDbSerializer _dbSerializer;
        private IDbConnection _connection;
        private string _dbFilePath;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _connection = TestHelper.CreateTempSQLiteDatabase();
            _dbFilePath = TestHelper.GetSQLiteDatabasePath(_connection.ConnectionString);
            _dbSerializer = new SQLiteSerializer(_connection);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _connection.Dispose();

            try
            {
                File.Delete(_dbFilePath);
            }
            catch
            {
                // dismiss
            }
        }

        [SetUp]
        public void SetUp()
        {
            _dbSerializer.Cruder.DbInspector.PurgeDb();
            IMigrator migrator = new Migrator(_connection.ConnectionString, Rdbms.SQLite, typeof(M0_Baseline).Assembly);
            migrator.Migrate();
        }

        [Test]
        public void SerializeDbMetadata_NoArguments_ProducesExpectedJson()
        {
            // Arrange

            // Act
            var json = _dbSerializer.SerializeDbMetadata();

            // Assert
            var expectedJson = this.GetType().Assembly.GetResourceText("expected-db-metadata.json", true);

            if (json != expectedJson)
            {
                TestHelper.WriteDiff(json, expectedJson, "c:/temp/0-meta", "json");
            }

            Assert.That(json, Is.EqualTo(expectedJson));
        }
    }
}
