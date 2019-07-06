using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Building.SqlServerCe;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.SqlServerCe;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Test.Utils.Building.SqlServerCe
{
    [TestFixture]
    public class SqlServerCeScriptBuilderTest : SqlServerCeTestBase
    {
        private IDbInspector _dbInspector;
        private IScriptBuilder _scriptBuilder;

        [SetUp]
        public void SetUp()
        {
            _dbInspector = new SqlServerCeInspector(this.Connection);
            _scriptBuilder = new SqlServerCeScriptBuilder
            {
                CurrentOpeningIdentifierDelimiter = '[',
            };
        }

        [Test]
        [TestCase("id", "[id] [bigint] NOT NULL IDENTITY(14, 88)")]
        [TestCase("login", "[login] [nvarchar](251) NOT NULL")]
        [TestCase("email", "[email] [nvarchar](252) NOT NULL")]
        [TestCase("password_hash", "[password_hash] [nvarchar](253) NOT NULL")]
        [TestCase("picture", "[picture] [image] NULL")]
        [TestCase("birthday", "[birthday] [datetime] NULL")]
        [TestCase("gender", "[gender] [bit] NOT NULL")]
        [TestCase("rating", "[rating] [numeric](10, 4) NULL")]
        [TestCase("fortune", "[fortune] [money] NOT NULL")]
        [TestCase("signature", "[signature] [varbinary](254) NOT NULL")]
        [TestCase("visual_age", "[visual_age] [tinyint] NULL")]
        [TestCase("alternate_rate", "[alternate_rate] [numeric](6, 1) NULL")]
        [TestCase("iq_index", "[iq_index] [float] NOT NULL")]
        [TestCase("index16", "[index16] [smallint] NULL")]
        [TestCase("index32", "[index32] [int] NOT NULL")]
        [TestCase("index64", "[index64] [bigint] NULL")]
        [TestCase("the_real", "[the_real] [real] NOT NULL")]
        [TestCase("guid", "[guid] [uniqueidentifier] NOT NULL")]
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
        [TestCase("secret", "ALTER TABLE [secret] ADD CONSTRAINT [PK_secret] PRIMARY KEY([id_base], [id_value])")]
        [TestCase(null, "CONSTRAINT [PK_secret] PRIMARY KEY([id_base], [id_value])")]
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
        [TestCase("IX_secret_name", "CREATE INDEX [IX_secret_name] ON [secret]([name])")]
        [TestCase("UX_secret_keyStart_keyEnd", "CREATE UNIQUE INDEX [UX_secret_keyStart_keyEnd] ON [secret]([key_start], [key_end])")]
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
            var expectedSql = this.GetType().Assembly.GetResourceText("sqlce-create-db-expected.sql", true);

            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        [Test]
        public void BuildFillDbSql_NoIncludedOrExcludedTables_ReturnsValidSql()
        {
            // Arrange
            var entireScript = this.GetType().Assembly.GetResourceText("sqlce-fill-db-all.sql", true);
            var insertingSqls = ScriptBuilderBase.SplitSqlByComments(entireScript);

            using (var command = this.Connection.CreateCommand())
            {
                foreach (var insertingSql in insertingSqls)
                {
                    command.CommandText = insertingSql;
                    command.ExecuteNonQuery();
                }
            }

            // Act
            var sql = _scriptBuilder.BuildFillDbSql(this.Connection);

            // Assert
            var expectedSql = this.GetType().Assembly.GetResourceText("sqlce-fill-db-expected-1.sql", true);

            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        [Test]
        public void BuildFillDbSql_IncludedTables_ReturnsValidSql()
        {
            // Arrange
            var entireScript = this.GetType().Assembly.GetResourceText("sqlce-fill-db-all.sql", true);
            var insertingSqls = ScriptBuilderBase.SplitSqlByComments(entireScript);

            using (var command = this.Connection.CreateCommand())
            {
                foreach (var insertingSql in insertingSqls)
                {
                    command.CommandText = insertingSql;
                    command.ExecuteNonQuery();
                }
            }

            // Act
            var sql = _scriptBuilder.BuildFillDbSql(
                this.Connection,
                tableNamesToInclude: new[]
                {
                    "fragment_sub_type",
                    "tag",
                    "fragment_type",
                    "language",
                });

            // Assert
            var expectedSql = this.GetType().Assembly.GetResourceText("sqlce-fill-db-expected-2.sql", true);

            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        [Test]
        public void BuildFillDbSql_ExcludedTables_ReturnsValidSql()
        {
            // Arrange
            var entireScript = this.GetType().Assembly.GetResourceText("sqlce-fill-db-all.sql", true);
            var insertingSqls = ScriptBuilderBase.SplitSqlByComments(entireScript);

            using (var command = this.Connection.CreateCommand())
            {
                foreach (var insertingSql in insertingSqls)
                {
                    command.CommandText = insertingSql;
                    command.ExecuteNonQuery();
                }
            }

            // Act
            var sql = _scriptBuilder.BuildFillDbSql(
                this.Connection,
                tableNamesToExclude: new[]
                {
                    "secret_detail",
                    "client",
                    "secret",
                    "color"
                });

            // Assert
            var expectedSql = this.GetType().Assembly.GetResourceText("sqlce-fill-db-expected-3.sql", true);

            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        [Test]
        public void BuildFillDbSql_IncludedAndExcludedTables_ThrowsArgumentException()
        {
            // Arrange
            var entireScript = this.GetType().Assembly.GetResourceText("sqlce-fill-db-all.sql", true);
            var insertingSqls = ScriptBuilderBase.SplitSqlByComments(entireScript);

            using (var command = this.Connection.CreateCommand())
            {
                foreach (var insertingSql in insertingSqls)
                {
                    command.CommandText = insertingSql;
                    command.ExecuteNonQuery();
                }
            }

            // Act
            var ex = Assert.Throws<ArgumentException>(() => _scriptBuilder.BuildFillDbSql(
                this.Connection,
                tableNamesToInclude: new string[0],
                tableNamesToExclude: new string[0]));

            // Assert
            Assert.That(ex.Message, Does.StartWith("Provide either 'tableNamesToInclude', or 'tableNamesToExclude', or neither of them, but not both"));
            Assert.That(ex.ParamName, Is.EqualTo("tableNamesToInclude/tableNamesToExclude"));
        }

        [Test]
        public void BuildClearDbSql_NoIncludedOrExcludedTables_ReturnsValidSql()
        {
            // Arrange

            // Act
            var sql = _scriptBuilder.BuildClearDbSql(this.Connection);

            // Assert
            var expectedSql = this.GetType().Assembly.GetResourceText("sqlce-clear-db-expected-1.sql", true);

            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        [Test]
        public void BuildClearDbSql_IncludedTables_ReturnsValidSql()
        {
            // Arrange

            // Act
            var sql = _scriptBuilder.BuildClearDbSql(this.Connection, tableNamesToInclude: new[] { "fragment", "user", });

            // Assert
            var expectedSql = this.GetType().Assembly.GetResourceText("sqlce-clear-db-expected-2.sql", true);

            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        [Test]
        public void BuildClearDbSql_ExcludedTables_ReturnsValidSql()
        {
            // Arrange

            // Act
            var sql = _scriptBuilder.BuildClearDbSql(this.Connection, tableNamesToExclude: new[] { "fragment", "user", });

            // Assert
            var expectedSql = this.GetType().Assembly.GetResourceText("sqlce-clear-db-expected-3.sql", true);

            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        [Test]
        public void BuildClearDbSql_BothIncludedAndExcludedTables_ReturnsValidSql()
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentException>(() => _scriptBuilder.BuildClearDbSql(
                this.Connection,
                tableNamesToInclude: new string[] { },
                tableNamesToExclude: new string[] { }));
        }

        [Test]
        public void BuildInsertSql_NotAllColumnsProvided_ReturnsValidSql()
        {
            // Arrange
            var tableInspector = _dbInspector.GetTableInspector("the_Sample");
            var table = tableInspector.GetTableMold();

            // Act
            var columnValues = new Dictionary<string, object>
            {
                { "id", 10 },
                { "color_id", 11 }
            };

            var sql = _scriptBuilder.BuildInsertSql(table, columnValues);

            // Assert
            Assert.That(sql, Is.EqualTo("INSERT INTO [the_sample]([id], [color_id]) VALUES(10, 11)"));
        }
    }
}
