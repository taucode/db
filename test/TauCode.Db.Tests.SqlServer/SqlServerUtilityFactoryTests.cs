using NUnit.Framework;
using System.Data;
using System.Data.SqlClient;
using TauCode.Db.SqlServer;

namespace TauCode.Db.Tests.SqlServer
{
    [TestFixture]
    public class SqlServerUtilityFactoryTests
    {
        [Test]
        public void Members_DifferentArguments_HaveExpectedProps()
        {
            // Arrange
            IUtilityFactory utilityFactory = SqlServerUtilityFactory.Instance;

            // Act
            var dbProviderName = utilityFactory.DbProviderName;

            //IDbConnection connection = utilityFactory.CreateConnection();
            IDbConnection connection = new SqlConnection();
            connection.ConnectionString = TestHelper.ConnectionString;
            connection.Open();

            IDialect dialect = utilityFactory.GetDialect();

            IScriptBuilder scriptBuilder = utilityFactory.CreateScriptBuilder();

            IDbInspector dbInspector = utilityFactory.CreateDbInspector(connection);

            ITableInspector tableInspector = utilityFactory.CreateTableInspector(connection, "language");

            ICruder cruder = utilityFactory.CreateCruder(connection);

            IDbSerializer dbSerializer = utilityFactory.CreateDbSerializer(connection);

            // Assert
            Assert.That(dbProviderName, Is.EqualTo("SqlServer"));
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
