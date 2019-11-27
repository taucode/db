using NUnit.Framework;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Db.Utils.Inspection.SQLite;

namespace TauCode.Db.Tests.Utils.Inspection.SQLite
{
    [TestFixture]
    public class SQLiteParserTests
    {
        [Test]
        public void Parse_ValidCreateIndexStatement_ProducesValidOutput()
        {
            // Arrange
            var sql = "CREATE UNIQUE INDEX \"UX_userInfo_taxNumber\" ON \"user_info\" (\"tax_number\" ASC)";
            var parser = SQLiteParser.Instance;

            // Act
            var results = parser.Parse(sql);

            // Assert
            var index = results.Cast<IndexMold>().Single();

            Assert.That(index.Name, Is.EqualTo("UX_userInfo_taxNumber"));
            Assert.That(index.TableName, Is.EqualTo("user_info"));

            var column = index.Columns.Single();

            Assert.That(column.Name, Is.EqualTo("tax_number"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));
        }
    }
}
