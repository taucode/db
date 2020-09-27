using NUnit.Framework;
using TauCode.Db;

// todo: get rid of '...SqlServer...' in sln.
namespace TauCode.Lab.Db.MySql.Tests
{
    [TestFixture]
    public class MySqlScriptBuilderTests : TestBase
    {
        private IDbScriptBuilder _scriptBuilder;

        [SetUp]
        public void SetUp()
        {
            _scriptBuilder = this.DbInspector.Factory.CreateScriptBuilder(null);
        }

        [Test]
        public void BuildCreateTableScript_ValidArgument_CreatesScript()
        {
            // Arrange
            var table = this.DbInspector
                .Factory
                .CreateTableInspector(this.Connection, null, "fragment")
                .GetTable();
            
            // Act
            var sql = _scriptBuilder.BuildCreateTableScript(table, true);

            // Assert
            var expectedSql = @"CREATE TABLE `fragment`(
    `id` char(36) NOT NULL,
    `note_translation_id` char(36) NOT NULL,
    `sub_type_id` char(36) NOT NULL,
    `code` varchar(255) NULL,
    `order` int NOT NULL,
    `content` text NOT NULL,
    CONSTRAINT `PRIMARY` PRIMARY KEY(`id` ASC),
    CONSTRAINT `FK_fragment_noteTranslation` FOREIGN KEY(`note_translation_id`) REFERENCES `note_translation`(`id`),
    CONSTRAINT `FK_fragment_subType` FOREIGN KEY(`sub_type_id`) REFERENCES `fragment_sub_type`(`id`))";

            if (sql != expectedSql)
            {
                TestHelper.WriteDiff(sql, expectedSql, @"C:\temp\0-opa", "sql", "todo");
            }

            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        protected override void ExecuteDbCreationScript()
        {
            var script = TestHelper.GetResourceText("rho.script-create-tables.sql");
            this.Connection.ExecuteCommentedScript(script);

        }
    }
}
