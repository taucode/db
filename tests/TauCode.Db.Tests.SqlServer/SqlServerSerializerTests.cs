using NUnit.Framework;
using TauCode.Db.SqlServer;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Tests.SqlServer
{
    [TestFixture]
    public class SqlServerSerializerTests : TestBase
    {
        private IDbSerializer _dbSerializer;

        [SetUp]
        public void SetUp()
        {
            _dbSerializer = new SqlServerSerializer(this.Connection);
        }

        [Test]
        public void SerializeTableMetadata_ValidInput_ProducesExpectedResult()
        {
            // Arrange

            // Act
            var json = _dbSerializer.SerializeTableMetadata("language");

            // Assert
            var expectedJson = this.GetType().Assembly.GetResourceText("metadata-language.json", true);

            if (json != expectedJson)
            {
                TestHelper.WriteDiff(json, expectedJson, "c:/temp/ko-33", ".json", "todo");
            }

            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [Test]
        public void SerializeTableData_ValidInput_ProducesExpectedResult()
        {
            // Arrange
            var insertScript = this.GetType().Assembly.GetResourceText("script-insert-data.sql", true);
            this.Connection.ExecuteScript(insertScript);

            // Act
            var json = _dbSerializer.SerializeTableData("language");

            // Assert
            var expectedJson = this.GetType().Assembly.GetResourceText("data-language.json", true);

            if (json != expectedJson)
            {
                TestHelper.WriteDiff(json, expectedJson, "c:/temp/ko-33", ".json", "todo");
            }

            Assert.That(json, Is.EqualTo(expectedJson));
        }
    }
}
