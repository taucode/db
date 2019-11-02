using NUnit.Framework;
using System;
using System.Linq;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Building.SQLite;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.SQLite;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Tests.Utils.Building.SQLite
{
    [TestFixture]
    public class SQLiteScriptBuilderTest : SQLiteTestBase
    {
        private IDbInspector _dbInspector;
        private IScriptBuilder _scriptBuilder;

        [SetUp]
        public void SetUp()
        {
            _dbInspector = new SQLiteInspector(this.Connection);
            _scriptBuilder = new SQLiteScriptBuilder
            {
                CurrentOpeningIdentifierDelimiter = '[',
            };
        }

        [Test]
        [TestCase("login", "[login] text NOT NULL")]
        [TestCase("email", "[email] text NOT NULL")]
        [TestCase("password_hash", "[password_hash] text NOT NULL")]
        [TestCase("picture", "[picture] blob NULL")]
        [TestCase("birthday", "[birthday] integer NULL")]
        [TestCase("gender", "[gender] integer NOT NULL")]
        [TestCase("rating", "[rating] numeric NULL")]
        [TestCase("fortune", "[fortune] numeric NOT NULL")]
        [TestCase("signature", "[signature] blob NOT NULL")]
        [TestCase("visual_age", "[visual_age] integer NULL")]
        [TestCase("alternate_rate", "[alternate_rate] numeric NULL")]
        [TestCase("iq_index", "[iq_index] real NOT NULL")]
        [TestCase("index16", "[index16] integer NULL")]
        [TestCase("index32", "[index32] integer NOT NULL")]
        [TestCase("index64", "[index64] integer NULL")]
        [TestCase("the_real", "[the_real] real NOT NULL")]
        [TestCase("guid", "[guid] blob NOT NULL")]
        public void BuildCreateColumnSql_ValidArguments_BuildsSql(string columnName, string expectedSql)
        {
            // Arrange
            var tableInspector = _dbInspector.GetTableInspector("client");
            var tableMold = tableInspector.GetTableMold();
            
            // Act
            var sql = _scriptBuilder.BuildCreateColumnSql(tableMold.Columns.Single(x => x.Name == columnName));

            // Assert
            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        [Test]
        [TestCase("secret", "ALTER TABLE [secret] ADD CONSTRAINT [PK_secret] PRIMARY KEY([id_base] ASC, [id_value] ASC)")]
        [TestCase(null, "CONSTRAINT [PK_secret] PRIMARY KEY([id_base] ASC, [id_value] ASC)")]
        public void BuildPrimaryKeySql_ValidArguments_BuildsSql(string tableName, string expectedSql)
        {
            // Arrange
            var tableInspector = _dbInspector.GetTableInspector("secret");
            var tableMold = tableInspector.GetTableMold();
            
            // Act
            var sql = _scriptBuilder.BuildPrimaryKeySql(tableName, tableMold.PrimaryKey);

            // Assert
            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        [Test]
        [TestCase("secret_detail",
@"ALTER TABLE [secret_detail] ADD CONSTRAINT [FK_secretDetail_secret] FOREIGN KEY([secret_id_base], [secret_id_value])
REFERENCES [secret]([id_base], [id_value])")]
        [TestCase(null, @"CONSTRAINT [FK_secretDetail_secret] FOREIGN KEY([secret_id_base], [secret_id_value]) REFERENCES [secret]([id_base], [id_value])")]
        public void BuildForeignKeySql_ValidArguments_BuildsSql(string tableName, string expectedSql)
        {
            // Arrange
            var tableInspector = _dbInspector.GetTableInspector("secret_detail");
            var tableMold = tableInspector.GetTableMold();
            var foreignKey = tableMold.ForeignKeys.Single(x => x.Name == "FK_secretDetail_secret");

            // Act
            var sql = _scriptBuilder.BuildForeignKeySql(tableName, foreignKey);

            // Assert
            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        [Test]
        [TestCase("IX_secret_name", "CREATE INDEX [IX_secret_name] ON [secret]([name] ASC)")]
        [TestCase("UX_secret_keyStart_keyEnd", "CREATE UNIQUE INDEX [UX_secret_keyStart_keyEnd] ON [secret]([key_start] ASC, [key_end] DESC)")]
        public void BuildIndexSql_ValidArguments_BuildsSql(string indexName, string expectedSql)
        {
            // Arrange
            var tableName = "secret";
            var tableInspector = _dbInspector.GetTableInspector(tableName);
            var tableMold = tableInspector.GetTableMold();
            var index = tableMold.Indexes.Single(x => x.Name == indexName);

            // Act
            var sql = _scriptBuilder.BuildIndexSql(tableName, index);

            // Assert
            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        [Test]
        public void BuildCreateDbSql_ValidArgument_ReturnsSql()
        {
            // Arrange

            // Act
            var sql =
                _scriptBuilder.BuildCreateDbSql(this.Connection, true) +
                Environment.NewLine;

            // Assert
            var expectedSql = this.GetType().Assembly.GetResourceText("sqlite-create-db-expected.sql", true);

            Assert.That(sql, Is.EqualTo(expectedSql));
        }
    }
}
