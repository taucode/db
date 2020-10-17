using NUnit.Framework;
using TauCode.Db;
using TauCode.Db.Exceptions;
using TauCode.Extensions;

namespace TauCode.Lab.Db.SqlClient.Tests.DbScriptBuilder
{
    [TestFixture]
    public class SqlScriptBuilderTests : TestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Connection.CreateSchema("zeta");

            var sql = this.GetType().Assembly.GetResourceText("crebase.sql", true);
            this.Connection.ExecuteCommentedScript(sql);
        }

        #region Constructor

        [Test]
        public void Constructor_SchemaIsNotNull_RunsOk()
        {
            // Arrange

            // Act
            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab("dbo");

            // Assert
            Assert.That(scriptBuilder.Connection, Is.Null);
            Assert.That(scriptBuilder.Factory, Is.EqualTo(SqlUtilityFactoryLab.Instance));
            Assert.That(scriptBuilder.SchemaName, Is.EqualTo("dbo"));
            Assert.That(scriptBuilder.CurrentOpeningIdentifierDelimiter, Is.EqualTo('['));
        }

        [Test]
        public void Constructor_SchemaIsNull_RunsOk()
        {
            // Arrange

            // Act
            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab(null);

            // Assert
            Assert.That(scriptBuilder.Connection, Is.Null);
            Assert.That(scriptBuilder.Factory, Is.EqualTo(SqlUtilityFactoryLab.Instance));
            Assert.That(scriptBuilder.SchemaName, Is.EqualTo("dbo"));
            Assert.That(scriptBuilder.CurrentOpeningIdentifierDelimiter, Is.EqualTo('['));
        }

        #endregion

        #region CurrentOpeningIdentifierDelimiter

        [Test]
        [TestCase('[')]
        [TestCase('"')]
        [TestCase(null)]
        public void CurrentOpeningIdentifierDelimiter_SetValidValue_ChangesValue(char? openingDelimiter)
        {
            // Arrange
            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab(null);

            // Act
            scriptBuilder.CurrentOpeningIdentifierDelimiter = openingDelimiter;

            // Assert
            Assert.That(scriptBuilder.CurrentOpeningIdentifierDelimiter, Is.EqualTo(openingDelimiter));
        }

        [Test]
        public void CurrentOpeningIdentifierDelimiter_SetInvalidValidValue_ThrowsTodo()
        {
            // Arrange
            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab(null);

            // Act
            var ex = Assert.Throws<TauDbException>(() => scriptBuilder.CurrentOpeningIdentifierDelimiter = '`');

            // Assert
            Assert.That(ex, Has.Message.EqualTo("Invalid opening identifier delimiter: '`'."));
        }

        #endregion

        #region BuildCreateTableScript

        [Test]
        public void BuildCreateTableScript_IncludeConstraints_ReturnsValidScript()
        {
            // Arrange
            IDbTableInspector tableInspector = new SqlTableInspectorLab(this.Connection, "zeta", "TaxInfo");
            var table = tableInspector.GetTable();

            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab("zeta");

            // Act
            var sql = scriptBuilder.BuildCreateTableScript(table, true);

            var expectedSql = @"
CREATE TABLE [zeta].[TaxInfo](
    [Id] [uniqueidentifier] NOT NULL,
    [PersonId] [bigint] NOT NULL,
    [Tax] [money] NOT NULL,
    [Ratio] [float] NULL,
    [PersonMetaKey] [smallint] NOT NULL,
    [SmallRatio] [real] NOT NULL,
    [RecordDate] [datetime] NULL,
    [CreatedAt] [datetimeoffset] NOT NULL,
    [PersonOrdNumber] [tinyint] NOT NULL,
    [DueDate] [datetime2] NULL,
    CONSTRAINT [PK_taxInfo] PRIMARY KEY([Id]),
    CONSTRAINT [FK_taxInfo_Person] FOREIGN KEY([PersonId], [PersonMetaKey], [PersonOrdNumber]) REFERENCES [zeta].[Person]([Id], [MetaKey], [OrdNumber]))
"
                .Trim();

            // Assert
            if (sql != expectedSql)
            {
                TestHelper.WriteDiff(sql, expectedSql, @"c:\temp\0-sql\", ".sql", "todo");
            }

            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        // - not include constraints
        // - table mold is null => throws
        // - corrupted table mold => throws TauDbException
        // - respects delimiter

        #endregion

        #region BuildCreateIndexScript

        // todo
        // - unique index
        // - non-unique index
        // - multi-column index with asc/desc
        // - index is null => throws
        // - index is corrupted => throws
        // - respects delimiter

        #endregion

        #region BuildDropTableScript

        // todo
        // - happy path
        // - respects delimiter
        // - arg is null => throws
        // - respects delimiter

        #endregion

        #region BuildInsertScript

        // todo
        // - happy path
        // - happy path: columnToParameterMappings is empty (=> default values)
        // - columnToParameterMappings has unknown columns => throws
        // - table is null => throws
        // - table is corrupted => throws
        // - columnToParameterMappings is null => throws
        // - columnToParameterMappings is corrupted => throws
        // - respects delimiter

        #endregion

        #region BuildUpdateScript

        // todo
        // - happy path
        // - columnToParameterMappings is empty => throws
        // - no pk => throws
        // - PK is multi-column => throws
        // - columnToParameterMappings does not contain PK column => throws
        // - table is null => throws
        // - table is corrupted => throws
        // - columnToParameterMappings is null => throws
        // - columnToParameterMappings is corrupted => throws
        // - respects delimiter

        #endregion

        #region BuildSelectByPrimaryKeyScript

        // todo
        // - happy path : respects columnSelector
        // - columnSelector is null => delivers all
        // - pk is absent => throws
        // - pk is multi-column => throws
        // - play around with columnSelector
        // - columnSelector returned 0 columns => throws
        // - respects delimiter

        #endregion

        #region BuildSelectAllScript

        // todo

        #endregion

        #region BuildDeleteByIdScript

        // todo

        #endregion

        #region BuildDeleteScript

        // todo

        #endregion
    }
}
