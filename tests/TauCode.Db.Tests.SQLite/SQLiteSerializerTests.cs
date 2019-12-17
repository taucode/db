using NUnit.Framework;
using System;
using System.Linq;
using TauCode.Db.Data;
using TauCode.Db.SQLite;

namespace TauCode.Db.Tests.SQLite
{
    [TestFixture]
    public class SQLiteSerializerTests : TestBase
    {
        private IDbSerializer _dbSerializer;

        [SetUp]
        public void SetUp()
        {
            _dbSerializer = new SQLiteSerializer(this.Connection);
            _dbSerializer.ScriptBuilder.CurrentOpeningIdentifierDelimiter = '[';
        }

        [Test]
        public void SerializeTableData_ValidInput_ProducesExpectedResult()
        {
            // Arrange
            var insertScript = TestHelper.GetResourceText("rho.script-insert-data.sql");
            this.Connection.ExecuteCommentedScript(insertScript);

            // Act
            var json = _dbSerializer.SerializeTableData("language");

            // Assert
            var expectedJson = TestHelper.GetResourceText("rho.data-language.json");

            if (json != expectedJson)
            {
                TestHelper.WriteDiff(json, expectedJson, "c:/temp/ko-33", ".json", "todo");
            }

            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [Test]
        public void SerializeDbData_ValidInput_ProducesExpectedResult()
        {
            // Arrange
            var insertScript = TestHelper.GetResourceText("rho.script-insert-data.sql");
            this.Connection.ExecuteCommentedScript(insertScript);

            // Act
            var json = _dbSerializer.SerializeDbData();

            // Assert
            var expectedJson = TestHelper.GetResourceText("rho.data-db.json");
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [Test]
        public void SerializeTableMetadata_ValidInput_ProducesExpectedResult()
        {
            // Arrange

            // Act
            var json = _dbSerializer.SerializeTableMetadata("language");

            // Assert
            var expectedJson = TestHelper.GetResourceText("rho.metadata-language.json");

            if (json != expectedJson)
            {
                TestHelper.WriteDiff(json, expectedJson, "c:/temp/ko-33", ".json", "todo");
            }

            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [Test]
        public void SerializeDbMetadata_ValidInput_ProducesExpectedResult()
        {
            // Arrange

            // Act
            var json = _dbSerializer.SerializeDbMetadata(x => !string.Equals(x, "versioninfo", StringComparison.InvariantCultureIgnoreCase));

            // Assert
            var expectedJson = TestHelper.GetResourceText(".rho.metadata-db.json");

            if (json != expectedJson)
            {
                TestHelper.WriteDiff(json, expectedJson, "c:/temp/ko-33", ".json", "todo");
            }

            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [Test]
        public void DeserializeTableData_ValidInput_ProducesExpectedResult()
        {
            // Arrange
            var json = TestHelper.GetResourceText("rho.data-language.json");

            // Act
            _dbSerializer.DeserializeTableData("language", json);

            // Assert
            var dictionary = this.GetRows("language").Select(x => new DynamicRow(x))
                .ToDictionary(x => x.GetValue("id"), x => (dynamic)x);

            var it = dictionary[new Guid("2a9ac9e3-eb27-4461-90d4-95a5e6b9d3e8")];
            Assert.That(it.code, Is.EqualTo("it"));
            Assert.That(it.name, Is.EqualTo("Italian"));

            var en = dictionary[new Guid("04990c0d-5d4a-41b9-98e4-103545d094d9")];
            Assert.That(en.code, Is.EqualTo("en"));
            Assert.That(en.name, Is.EqualTo("English"));
        }

        protected override void ExecuteDbCreationScript()
        {
            var migrator = new TestMigrator(this.ConnectionString, this.GetType().Assembly);
            migrator.Migrate();
        }
    }
}
