using NUnit.Framework;
using System.Data;
using System.Data.SqlClient;
using TauCode.Db.SqlServer;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Tests.SqlServer
{
    [TestFixture]
    public class SqlServerInspectorTests
    {
        private IDbInspector _dbInspector;
        private IDbConnection _connection;

        private const string ConnectionString = @"Server=.\mssqltest;Database=rho.test;User Id=testadmin;Password=1234;";

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _connection = new SqlConnection(ConnectionString);
            _connection.Open();

            _dbInspector = new SqlServerInspector(_connection);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _connection.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            this.DropTables();
            this.CreateTables();
        }

        private void CreateTables()
        {
            var script = this.GetType().Assembly.GetResourceText("create-tables.sql", true);
            var sqls = DbUtils.SplitScriptByComments(script);

            foreach (var sql in sqls)
            {
                DbUtils.ExecuteSql(_connection, sql);
            }
        }

        private void DropTables()
        {
            var script = this.GetType().Assembly.GetResourceText("drop-tables.sql", true);
            var sqls = DbUtils.SplitScriptByComments(script);

            foreach (var sql in sqls)
            {
                try
                {
                    DbUtils.ExecuteSql(_connection, sql);
                }
                catch
                {
                    // ignore exception - maybe table does not exist yet.
                }
            }
        }

        [Test]
        public void GetTableNames_IndependentFirstIsNull_ReturnsIndependentTablesFirst()
        {
            // Arrange

            // Act
            var tableNames = _dbInspector.GetTableNames();

            // Assert
            CollectionAssert.AreEquivalent(
                new[]
                {
                    "fragment_type",
                    "language",
                    "note",
                    "tag",
                    "user",
                    "fragment_sub_type",
                    "note_tag",
                    "note_translation",
                    "fragment",
                },
                tableNames);
        }

        [Test]
        public void GetTableNames_IndependentFirstIsTrue_ReturnsIndependentTablesFirst()
        {
            // Arrange

            // Act
            var tableNames = _dbInspector.GetTableNames(true);

            // Assert
            CollectionAssert.AreEqual(
                new[]
                {
                    "fragment_type",
                    "language",
                    "note",
                    "tag",
                    "user",
                    "fragment_sub_type",
                    "note_tag",
                    "note_translation",
                    "fragment",
                },
                tableNames);
        }

        [Test]
        public void GetTableNames_IndependentFirstIsFalse_ReturnsDependentTablesFirst()
        {
            // Arrange

            // Act
            var tableNames = _dbInspector.GetTableNames(false);

            // Assert
            CollectionAssert.AreEqual(
                new[]
                {
                    "fragment",
                    "fragment_sub_type",
                    "note_tag",
                    "note_translation",
                    "fragment_type",
                    "language",
                    "note",
                    "tag",
                    "user",
                },
                tableNames);
        }
    }
}
