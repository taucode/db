using NUnit.Framework;
using System.Collections.Generic;
using TauCode.Db.Model;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Building.SQLite;
using TauCode.Db.Utils.Parsing;
using TauCode.Db.Utils.Parsing.SQLite;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Test.Utils.Parsing.SQLite
{
    [TestFixture]
    public class SQLiteScriptParserTest
    {
        private IScriptParser _parser;
        private IScriptBuilder _builder;

        [SetUp]
        public void SetUp()
        {
            _parser = new SQLiteScriptParser();
            _builder = new SQLiteScriptBuilder
            {
                CurrentOpeningIdentifierDelimiter = '['
            };
        }

        [Test]
        public void Parse_ValidScript_ReturnsExpectedSql()
        {
            // Arrange
            var entireScript = this.GetType().Assembly.GetResourceText("parse-sqlite.sql", true);
            var splittedScripts = ScriptBuilderBase.SplitSqlByComments(entireScript);

            var builtSqls = new List<string>();
            var expectedSqls = new List<string>();

            // Act
            for (var i = 0; i < splittedScripts.Length / 2; i++)
            {
                var originalSql = splittedScripts[i * 2];
                var parsedTable = (TableMold)_parser.Parse(originalSql)[0];
                var builtSql = _builder.BuildCreateTableSql(parsedTable, true);

                var expectedSql = splittedScripts[i * 2 + 1];

                builtSqls.Add(builtSql);
                expectedSqls.Add(expectedSql);
            }

            // Assert
            Assert.That(builtSqls.Count, Is.EqualTo(expectedSqls.Count));

            for (var i = 0; i < builtSqls.Count; i++)
            {
                var sql = builtSqls[i];
                var expectedSql = expectedSqls[i];

                Assert.That(sql, Is.EqualTo(expectedSql));
            }
        }
    }
}
