using NUnit.Framework;
using System;
using System.Linq;
using TauCode.Db.Exceptions;
using TauCode.Db.Utils.Inspection.SqlServerCe;

namespace TauCode.Db.Test.Utils.Inspection.SqlServerCe
{
    [TestFixture]
    public class SqlServerCeInspectorTest : SqlServerCeTestBase
    {
        [Test]
        public void GetTableNames_NoArguments_ReturnsTableNames()
        {
            // Arrange
            var db = new SqlServerCeInspector(this.Connection);

            // Act
            var tableNames = db
                .GetTableNames()
                .OrderBy(x => x);

            // Assert
            var expectedTableNames = new[]
                {
                    "the_sample",
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

        [Test]
        public void GetTableInspector_NonExistingTable_ThrowsObjectNotFoundException()
        {
            // Arrange
            var db = new SqlServerCeInspector(this.Connection);

            // Act & Assert
            var ex = Assert.Throws<ObjectNotFoundException>(() => db.GetTableInspector("non_existing_table"));

            Assert.That(ex.Message, Does.StartWith("Table not found: 'non_existing_table'"));
        }

        [Test]
        public void GetTableInspector_ArgumentIsNull_ThrowsObjectNotFoundException()
        {
            // Arrange
            var db = new SqlServerCeInspector(this.Connection);

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => db.GetTableInspector(null));

            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }
    }
}
