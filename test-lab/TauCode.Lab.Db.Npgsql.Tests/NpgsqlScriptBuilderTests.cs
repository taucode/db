using NUnit.Framework;
using TauCode.Db;

namespace TauCode.Lab.Db.Npgsql.Tests
{
    [TestFixture]
    public class NpgsqlScriptBuilderTests : TestBase
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
            var expectedSql = @"CREATE TABLE ""fragment""(
    ""id"" uuid NOT NULL,
    ""note_translation_id"" uuid NOT NULL,
    ""sub_type_id"" uuid NOT NULL,
    ""code"" character varying(255) NULL,
    ""order"" integer NOT NULL,
    ""content"" text NOT NULL,
    CONSTRAINT ""PK_fragment"" PRIMARY KEY(""id""),
    CONSTRAINT ""FK_fragment_noteTranslation"" FOREIGN KEY(""note_translation_id"") REFERENCES ""note_translation""(""id""),
    CONSTRAINT ""FK_fragment_subType"" FOREIGN KEY(""sub_type_id"") REFERENCES ""fragment_sub_type""(""id""))";

            if (sql != expectedSql)
            {
                TestHelper.WriteDiff(sql, expectedSql, @"c:\temp\0-ka", "sql", "todo");
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
