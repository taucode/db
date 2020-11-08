using MySql.Data.MySqlClient;
using NUnit.Framework;
using System;
using TauCode.Db;
using TauCode.Db.Exceptions;

// todo clean up
namespace TauCode.Lab.Db.MySql.Tests.DbInspector
{
    [TestFixture]
    public class MySqlInspectorTestsLab : TestBase
    {
        #region Constructor

        /// <summary>
        /// Creates MySqlInspector with valid connection and existing schema
        /// </summary>
        [Test]
        public void Constructor_ValidArguments_RunsOk()
        {
            // Arrange

            // Act
            IDbInspector inspector = new MySqlInspectorLab(this.Connection);

            // Assert
            Assert.That(inspector.Connection, Is.SameAs(this.Connection));
            Assert.That(inspector.Factory, Is.SameAs(MySqlUtilityFactoryLab.Instance));

            Assert.That(inspector.SchemaName, Is.EqualTo("foo"));
        }

        [Test]
        public void Constructor_SchemaIsNull_RunsOkAndSchemaIsFoo()
        {
            // Arrange

            // Act
            IDbInspector inspector = new MySqlInspectorLab(this.Connection);

            // Assert
            Assert.That(inspector.Connection, Is.SameAs(this.Connection));
            Assert.That(inspector.Factory, Is.SameAs(MySqlUtilityFactoryLab.Instance));

            Assert.That(inspector.SchemaName, Is.EqualTo("foo"));
        }

        [Test]
        public void Constructor_ConnectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new MySqlInspectorLab(null));
            
            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
        }

        [Test]
        public void Constructor_ConnectionIsNotOpen_ThrowsArgumentException()
        {
            // Arrange
            using var connection = new MySqlConnection(TestHelper.ConnectionString);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => new MySqlInspectorLab(connection));

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

            IDbInspector inspector = new MySqlInspectorLab(this.Connection);

            // Act
            var schemaNames = inspector.GetSchemaNames();

            // Assert
            CollectionAssert.AreEqual(
                new []
                {
                    "foo",
                    "hangfire",
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
CREATE TABLE `zeta`.`Tab2`(`id` int PRIMARY KEY)
");

            this.Connection.ExecuteSingleSql(@"
CREATE TABLE `zeta`.`Tab1`(`id` int PRIMARY KEY)
");

            this.Connection.ExecuteSingleSql(@"
CREATE TABLE `foo`.`Tab3`(`id` int PRIMARY KEY)
");
            using var connection = TestHelper.CreateConnection("zeta");

            IDbInspector inspector = new MySqlInspectorLab(connection);

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
            this.Connection.CreateSchema("kappa");

            this.Connection.ExecuteSingleSql(@"
CREATE TABLE `zeta`.`tab2`(`id` int PRIMARY KEY)
");

            this.Connection.ExecuteSingleSql(@"
CREATE TABLE `zeta`.`tab1`(`id` int PRIMARY KEY)
");

            this.Connection.ExecuteSingleSql(@"
CREATE TABLE `foo`.`tab3`(`id` int PRIMARY KEY)
");

            using var connection = TestHelper.CreateConnection("kappa");
            this.Connection.DropSchema("kappa");

            IDbInspector inspector = new MySqlInspectorLab(connection);

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetTableNames());
            
            // Assert
            Assert.That(ex, Has.Message.EqualTo("Schema 'kappa' does not exist."));
        }

        #endregion
    }
}
