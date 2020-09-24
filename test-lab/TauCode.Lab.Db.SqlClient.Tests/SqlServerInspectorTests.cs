using NUnit.Framework;
using System.Linq;
using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient.Tests
{
    [TestFixture]
    public class SqlServerInspectorTests : TestBase
    {
        [Test]
        public void GetTableNames_IndependentFirstIsNull_ReturnsIndependentTablesFirst()
        {
            // Arrange

            // Act
            var tableNames = this.DbInspector.GetTableNames().Except(new[] { "foo" });

            // Assert
            CollectionAssert.AreEquivalent(
                new[]
                {
                    "fragment_type",
                    "language",
                    "note",
                    "tag",
                    "user",
                    "fragment_sub_type",
                    "note_tag",
                    "note_translation",
                    "fragment",
                },
                tableNames);
        }

        [Test]
        public void GetTableNames_IndependentFirstIsTrue_ReturnsIndependentTablesFirst()
        {
            // Arrange

            // Act
            var tableNames = this.DbInspector.GetTableNames(true).Except(new[] { "foo" });

            // Assert
            CollectionAssert.AreEqual(
                new[]
                {
                    "fragment_type",
                    "language",
                    "note",
                    "tag",
                    "user",
                    "fragment_sub_type",
                    "note_tag",
                    "note_translation",
                    "fragment",
                },
                tableNames);
        }

        [Test]
        public void GetTableNames_IndependentFirstIsFalse_ReturnsDependentTablesFirst()
        {
            // Arrange

            // Act
            var tableNames = this.DbInspector.GetTableNames(false).Except(new[] { "foo" });

            // Assert
            CollectionAssert.AreEqual(
                new[]
                {
                    "fragment",
                    "fragment_sub_type",
                    "note_tag",
                    "note_translation",
                    "fragment_type",
                    "language",
                    "note",
                    "tag",
                    "user",
                },
                tableNames);
        }

        protected override void ExecuteDbCreationScript()
        {
            var script = TestHelper.GetResourceText("rho.script-create-tables.sql");
            this.Connection.ExecuteCommentedScript(script);
        }
    }
}
