using Microsoft.Data.SqlClient;
using NUnit.Framework;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient.Tests
{
    [TestFixture]
    public class SqlServerUtilityFactoryTests
    {
        [Test]
        public void Members_DifferentArguments_HaveExpectedProps()
        {
            // Arrange
            IDbUtilityFactory utilityFactory = SqlUtilityFactory.Instance;

            // Act
            //var dbProviderName = utilityFactory.DbProviderName;

            //IDbConnection connection = utilityFactory.CreateConnection();
            IDbConnection connection = new SqlConnection();
            connection.ConnectionString = TestHelper.ConnectionString;
            connection.Open();

            IDbDialect dialect = utilityFactory.GetDialect();

            IDbScriptBuilder scriptBuilder = utilityFactory.CreateScriptBuilder();

            IDbInspector dbInspector = utilityFactory.CreateDbInspector(connection);

            IDbTableInspector tableInspector = utilityFactory.CreateTableInspector(connection, "language");

            IDbCruder cruder = utilityFactory.CreateCruder(connection);

            IDbSerializer dbSerializer = utilityFactory.CreateDbSerializer(connection);

            // Assert
            //Assert.That(dbProviderName, Is.EqualTo("SqlServer"));
            Assert.That(connection, Is.TypeOf<SqlConnection>());
            Assert.That(dialect, Is.SameAs(SqlDialect.Instance));

            Assert.That(scriptBuilder, Is.TypeOf<SqlScriptBuilder>());
            Assert.That(scriptBuilder.CurrentOpeningIdentifierDelimiter, Is.EqualTo('['));

            Assert.That(dbInspector, Is.TypeOf<SqlInspector>());
            Assert.That(tableInspector, Is.TypeOf<SqlTableInspector>());
            Assert.That(cruder, Is.TypeOf<SqlCruder>());
            Assert.That(dbSerializer, Is.TypeOf<SqlSerializer>());
        }
    }
}
