﻿using NUnit.Framework;

namespace TauCode.Db.Tests.SqlServer
{
    [TestFixture]
    public class SqlServerScriptBuilderLabTests : TestBase
    {
        private IScriptBuilderLab _scriptBuilderLab;

        [SetUp]
        public void SetUp()
        {
            _scriptBuilderLab = this.DbInspector.Factory.CreateScriptBuilderLab();
            _scriptBuilderLab.CurrentOpeningIdentifierDelimiter = '[';
        }

        [Test]
        public void BuildCreateTableScript_ValidArgument_CreatesScript()
        {
            // Arrange
            var table = this.DbInspector
                .Factory
                .CreateTableInspector(this.Connection, "fragment")
                .GetTable();
            
            // Act
            var sql = _scriptBuilderLab.BuildCreateTableScript(table, true);

            // Assert
            var expectedSql = @"CREATE TABLE [fragment](
    [id] [uniqueidentifier] NOT NULL,
    [note_translation_id] [uniqueidentifier] NOT NULL,
    [sub_type_id] [uniqueidentifier] NOT NULL,
    [code] [nvarchar](255) NULL,
    [order] [int] NOT NULL,
    [content] [ntext] NOT NULL,
    CONSTRAINT [PK_fragment] PRIMARY KEY([id] ASC),
    CONSTRAINT [FK_fragment_noteTranslation] FOREIGN KEY([note_translation_id]) REFERENCES [note_translation]([id]),
    CONSTRAINT [FK_fragment_subType] FOREIGN KEY([sub_type_id]) REFERENCES [fragment_sub_type]([id]))";

            if (sql != expectedSql)
            {
                TestHelper.WriteDiff(sql, expectedSql, "c:/temp/wazze0", ".sql", "todo");
            }

            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        protected override void ExecuteDbCreationScript()
        {
            throw new System.NotImplementedException();
        }
    }
}
