using NUnit.Framework;
using System;
using System.Data.SQLite;
using TauCode.Db;
using TauCode.Db.Exceptions;

namespace TauCode.Lab.Db.SQLite.Tests.DbInspector
{
    [TestFixture]
    public class SQLiteInspectorTestsLab : TestBase
    {
        #region Constructor

        /// <summary>
        /// Creates SqlInspector with valid connection and existing schema
        /// </summary>
        [Test]
        public void Constructor_ValidArguments_RunsOk()
        {
            // Arrange

            // Act
            IDbInspector inspector = new SQLiteInspectorLab(this.Connection, "dbo");

            // Assert
            Assert.That(inspector.Connection, Is.SameAs(this.Connection));
            Assert.That(inspector.Factory, Is.SameAs(SQLiteUtilityFactoryLab.Instance));

            Assert.That(inspector.SchemaName, Is.EqualTo("dbo"));
        }

        [Test]
        public void Constructor_SchemaIsNull_RunsOkAndSchemaIsDbo()
        {
            // Arrange

            // Act
            IDbInspector inspector = new SQLiteInspectorLab(this.Connection, null);

            // Assert
            Assert.That(inspector.Connection, Is.SameAs(this.Connection));
            Assert.That(inspector.Factory, Is.SameAs(SQLiteUtilityFactoryLab.Instance));

            Assert.That(inspector.SchemaName, Is.EqualTo("dbo"));
        }

        [Test]
        public void Constructor_ConnectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new SQLiteInspectorLab(null, "dbo"));
            
            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
        }

        [Test]
        public void Constructor_ConnectionIsNotOpen_ThrowsArgumentException()
        {
            // Arrange
            using var connection = new SQLiteConnection(TestHelper.ConnectionString);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => new SQLiteInspectorLab(connection, "dbo"));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("Connection should be opened."));
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
        }

        #endregion

        #region GetSchemaNames

        [Test]
        public void GetSchemaNames_NoArguments_ReturnsSchemaNames()
        {
            // Arrange
            IDbInspector inspector = new SQLiteInspectorLab(this.Connection, "dbo");

            // Act
            var schemaNames = inspector.GetSchemaNames();

            // Assert
            CollectionAssert.AreEqual(
                new []
                {
                    "dbo",
                    "HangFire",
                    "hello",
                    "zeta",
                },
                schemaNames);
        }

        #endregion

        #region GetTableNames

        [Test]
        public void GetTableNames_ExistingSchema_ReturnsTableNames()
        {
            // Arrange
            this.Connection.ExecuteSingleSql(@"
CREATE TABLE [zeta].[tab2]([id] int PRIMARY KEY)
");

            this.Connection.ExecuteSingleSql(@"
CREATE TABLE [zeta].[tab1]([id] int PRIMARY KEY)
");

            this.Connection.ExecuteSingleSql(@"
CREATE TABLE [dbo].[tab3]([id] int PRIMARY KEY)
");


            IDbInspector inspector = new SQLiteInspectorLab(this.Connection, "zeta");

            // Act
            var tableNames = inspector.GetTableNames();

            // Assert
            CollectionAssert.AreEqual(
                new[]
                {
                    "tab1",
                    "tab2",
                },
                tableNames);
        }

        [Test]
        public void GetTableNames_NonExistingSchema_ReturnsEmptyList()
        {
            // Arrange

            this.Connection.ExecuteSingleSql(@"
CREATE TABLE [zeta].[tab2]([id] int PRIMARY KEY)
");

            this.Connection.ExecuteSingleSql(@"
CREATE TABLE [zeta].[tab1]([id] int PRIMARY KEY)
");

            this.Connection.ExecuteSingleSql(@"
CREATE TABLE [dbo].[tab3]([id] int PRIMARY KEY)
");

            IDbInspector inspector = new SQLiteInspectorLab(this.Connection, "kappa");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetTableNames());
            
            // Assert
            Assert.That(ex, Has.Message.EqualTo("Schema 'kappa' does not exist."));
        }

        #endregion
    }
}
