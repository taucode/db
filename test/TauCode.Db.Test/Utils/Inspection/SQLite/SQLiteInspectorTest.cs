using NUnit.Framework;
using System.Linq;
using TauCode.Db.Utils.Inspection.SQLite;

namespace TauCode.Db.Test.Utils.Inspection.SQLite
{
    [TestFixture]
    public class SQLiteInspectorTest : SQLiteTestBase
    {
        [Test]
        public void GetTableNames_NoArguments_ReturnsTableNames()
        {
            // Arrange
            var db = new SQLiteInspector(this.Connection);

            // Act
            var tableNames = db
                .GetTableNames()
                .OrderBy(x => x);

            // Assert
            var expectedTableNames = new[]
                {
                    "user",
                    "language",
                    "fragment_type",
                    "fragment_sub_type",
                    "tag",
                    "note",
                    "note_tag",
                    "note_translation",
                    "fragment",
                    "client",
                    "color",
                    "secret",
                    "secret_detail"
                }
                .OrderBy(x => x);

            CollectionAssert.AreEqual(expectedTableNames, tableNames);
        }

        protected override string ResourceName => "sqlite-create-db.sql";
    }
}
