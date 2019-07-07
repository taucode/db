using NUnit.Framework;
using TauCode.Db.Model;
using TauCode.Db.Utils.Parsing;
using TauCode.Db.Utils.Parsing.Ansi;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Test.Utils.Parsing.Ansi
{
    [TestFixture]
    public class AnsiScriptParserTest
    {
        [Test]
        public void Parse_ValidScript_ReturnsTableMold()
        {
            // Arrange
            IScriptParser parser = new AnsiScriptParser();
            var script = this.GetType().Assembly.GetResourceText("parse-ansi.sql", true);

            // Act
            var results = parser.Parse(script);

            // Assert
            Assert.That(results, Has.Length.EqualTo(1));

            var table = results[0] as TableMold;
            Assert.That(table, Is.Not.Null);

            Assert.That(table.Name, Is.EqualTo("the_user"));

            Assert.That(table.Columns, Has.Count.EqualTo(6));

            var column = table.Columns[0];
            Assert.That(column.Name, Is.EqualTo("id"));
            Assert.That(column.IsNullable, Is.False);
            Assert.That(column.Type.Name, Is.EqualTo("integer"));
            Assert.That(column.Type.Size, Is.Null);

            column = table.Columns[1];
            Assert.That(column.Name, Is.EqualTo("the_login"));
            Assert.That(column.IsNullable, Is.False);
            Assert.That(column.Type.Name, Is.EqualTo("varchar"));
            Assert.That(column.Type.Size, Is.EqualTo(20));

            column = table.Columns[2];
            Assert.That(column.Name, Is.EqualTo("email"));
            Assert.That(column.IsNullable, Is.False);
            Assert.That(column.Type.Name, Is.EqualTo("int"));
            Assert.That(column.Type.Size, Is.Null);

            column = table.Columns[3];
            Assert.That(column.Name, Is.EqualTo("password_hash"));
            Assert.That(column.IsNullable, Is.False);
            Assert.That(column.Type.Name, Is.EqualTo("char"));
            Assert.That(column.Type.Size, Is.EqualTo(30));

            column = table.Columns[4];
            Assert.That(column.Name, Is.EqualTo("optional1"));
            Assert.That(column.IsNullable, Is.True);
            Assert.That(column.Type.Name, Is.EqualTo("bigint"));
            Assert.That(column.Type.Size, Is.Null);

            column = table.Columns[5];
            Assert.That(column.Name, Is.EqualTo("optional2"));
            Assert.That(column.IsNullable, Is.False);
            Assert.That(column.Type.Name, Is.EqualTo("int"));
            Assert.That(column.Type.Size, Is.Null);
        }
    }
}
