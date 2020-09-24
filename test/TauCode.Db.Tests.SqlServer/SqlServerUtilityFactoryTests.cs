using NUnit.Framework;
using System.Data;
using System.Data.SqlClient;
using TauCode.Lab.Db.SqlClient;

namespace TauCode.Db.Tests.SqlServer
{
    [TestFixture]
    public class SqlServerUtilityFactoryTests
    {
        [Test]
        public void Members_DifferentArguments_HaveExpectedProps()
        {
            // Arrange
            IDbUtilityFactory utilityFactory = SqlServerUtilityFactory.Instance;

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
            Assert.That(dialect, Is.SameAs(SqlServerDialect.Instance));

            Assert.That(scriptBuilder, Is.TypeOf<SqlServerScriptBuilder>());
            Assert.That(scriptBuilder.CurrentOpeningIdentifierDelimiter, Is.EqualTo('['));

            Assert.That(dbInspector, Is.TypeOf<SqlServerInspector>());
            Assert.That(tableInspector, Is.TypeOf<SqlServerTableInspector>());
            Assert.That(cruder, Is.TypeOf<SqlServerCruder>());
            Assert.That(dbSerializer, Is.TypeOf<SqlServerSerializer>());
        }
    }
}
