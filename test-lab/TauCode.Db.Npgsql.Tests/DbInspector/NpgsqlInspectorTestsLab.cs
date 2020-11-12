using System;
using Npgsql;
using NUnit.Framework;
using TauCode.Db.Exceptions;

namespace TauCode.Db.Npgsql.Tests.DbInspector
{
    [TestFixture]
    public class NpgsqlInspectorTestsLab : TestBase
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
            IDbInspector inspector = new NpgsqlInspector(this.Connection, "public");

            // Assert
            Assert.That(inspector.Connection, Is.SameAs(this.Connection));
            Assert.That(inspector.Factory, Is.SameAs(NpgsqlUtilityFactory.Instance));

            Assert.That(inspector.SchemaName, Is.EqualTo("public"));
        }

        [Test]
        public void Constructor_SchemaIsNull_RunsOkAndSchemaIsPublic()
        {
            // Arrange

            // Act
            IDbInspector inspector = new NpgsqlInspector(this.Connection, null);

            // Assert
            Assert.That(inspector.Connection, Is.SameAs(this.Connection));
            Assert.That(inspector.Factory, Is.SameAs(NpgsqlUtilityFactory.Instance));

            Assert.That(inspector.SchemaName, Is.EqualTo("public"));
        }

        [Test]
        public void Constructor_ConnectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new NpgsqlInspector(null, "public"));
            
            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
        }

        [Test]
        public void Constructor_ConnectionIsNotOpen_ThrowsArgumentException()
        {
            // Arrange
            using var connection = new NpgsqlConnection(TestHelper.ConnectionString);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => new NpgsqlInspector(connection, "public"));

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
            this.Connection.CreateSchema("zeta");
            this.Connection.CreateSchema("hello");
            this.Connection.CreateSchema("HangFire");

            IDbInspector inspector = new NpgsqlInspector(this.Connection, "public");

            // Act
            var schemaNames = inspector.GetSchemaNames();

            // Assert
            CollectionAssert.AreEquivalent(
                new []
                {
                    "public",
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
            this.Connection.CreateSchema("zeta");

            this.Connection.ExecuteSingleSql(@"
CREATE TABLE ""zeta"".""tab2""(""id"" int PRIMARY KEY)
");

            this.Connection.ExecuteSingleSql(@"
CREATE TABLE ""zeta"".""tab1""(""id"" int PRIMARY KEY)
");

            this.Connection.ExecuteSingleSql(@"
CREATE TABLE ""public"".""tab3""(""id"" int PRIMARY KEY)
");


            IDbInspector inspector = new NpgsqlInspector(this.Connection, "zeta");

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
            this.Connection.CreateSchema("zeta");

            this.Connection.ExecuteSingleSql(@"
CREATE TABLE ""zeta"".""tab2""(""id"" int PRIMARY KEY)
");

            this.Connection.ExecuteSingleSql(@"
CREATE TABLE ""zeta"".""tab1""(""id"" int PRIMARY KEY)
");

            this.Connection.ExecuteSingleSql(@"
CREATE TABLE ""public"".""tab3""(""id"" int PRIMARY KEY)
");

            IDbInspector inspector = new NpgsqlInspector(this.Connection, "kappa");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetTableNames());
            
            // Assert
            Assert.That(ex, Has.Message.EqualTo("Schema 'kappa' does not exist."));
        }

        #endregion
    }
}
