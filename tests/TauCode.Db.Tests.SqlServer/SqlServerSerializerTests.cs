using NUnit.Framework;
using System;
using System.Linq;
using TauCode.Db.Data;
using TauCode.Db.SqlServer;
using TauCode.Db.Tests.Common;
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
        public void SerializeTableData_ValidInput_ProducesExpectedResult()
        {
            // Arrange
            var insertScript = CommonTestHelper.GetResourceText("rho.script-insert-data.sql");
            this.Connection.ExecuteCommentedScript(insertScript);

            // Act
            var json = _dbSerializer.SerializeTableData("language");

            // Assert
            var expectedJson = CommonTestHelper.GetResourceText("rho.data-language.json");

            if (json != expectedJson)
            {
                TestHelper.WriteDiff(json, expectedJson, "c:/temp/ko-33", ".json", "todo");
            }

            Assert.That(json, Is.EqualTo(expectedJson));
        }
        
        [Test]
        public void SerializeTableMetadata_ValidInput_ProducesExpectedResult()
        {
            // Arrange

            // Act
            var json = _dbSerializer.SerializeTableMetadata("language");

            // Assert
            var expectedJson = CommonTestHelper.GetResourceText("rho.metadata-language.json");

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
            var json = CommonTestHelper.GetResourceText("rho.data-language.json");
            
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
    }
}
