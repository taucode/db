using NUnit.Framework;
using System;
using System.Linq;

namespace TauCode.Db.Tests.SQLite
{
    [TestFixture]
    public class SQLiteInspectorTests : TestBase
    {
        [Test]
        public void GetTableNames_IndependentFirstIsNull_ReturnsIndependentTablesFirst()
        {
            // Arrange

            // Act
            var tableNames = this.DbInspector
                .GetTableNames()
                .Except(new[] { "versioninfo", "foo", "hoo" }, StringComparer.InvariantCultureIgnoreCase);

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
            var tableNames = this.DbInspector
                .GetTableNames(true)
                .Except(new[] { "versioninfo", "foo", "hoo" }, StringComparer.InvariantCultureIgnoreCase);

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
            var tableNames = this.DbInspector
                .GetTableNames(false)
                .Except(new[] { "versioninfo", "foo", "hoo" }, StringComparer.InvariantCultureIgnoreCase);

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
            var migrator = new TestMigrator(this.ConnectionString, this.GetType().Assembly);
            migrator.Migrate();
        }
    }
}
