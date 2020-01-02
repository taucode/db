using NUnit.Framework;
using TauCode.Db.Exceptions;

namespace TauCode.Db.Tests.SqlServer
{
    /// <summary>
    /// Happy paths are successfully passed in other ut-s.
    /// </summary>
    [TestFixture]
    public class SqlServerTableInspectorTests : TestBase
    {
        [SetUp]
        public void SetUp()
        {
        }

        [Test]
        public void GetTable_NonExistingTable_ThrowsDbException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<DbException>(() =>
                {
                    var tableInspector = this.DbInspector.Factory.CreateTableInspector(this.Connection, "non_existing_table");
                    tableInspector.GetTable();
                });

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table not found: 'non_existing_table'."));
        }

        protected override void ExecuteDbCreationScript()
        {
            var script = TestHelper.GetResourceText("rho.script-create-tables.sql");
            this.Connection.ExecuteCommentedScript(script);
        }
    }
}
