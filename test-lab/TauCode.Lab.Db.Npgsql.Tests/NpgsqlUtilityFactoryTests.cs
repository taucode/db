using Npgsql;
using NUnit.Framework;
using System.Data;
using TauCode.Db;

namespace TauCode.Lab.Db.Npgsql.Tests
{
    [TestFixture]
    public class NpgsqlUtilityFactoryTests
    {
        [Test]
        public void Members_DifferentArguments_HaveExpectedProps()
        {
            // Arrange
            IDbUtilityFactory utilityFactory = NpgsqlUtilityFactory.Instance;

            // Act
            IDbConnection connection = new NpgsqlConnection();
            connection.ConnectionString = TestHelper.ConnectionString;
            connection.Open();

            IDbDialect dialect = utilityFactory.GetDialect();

            IDbScriptBuilder scriptBuilder = utilityFactory.CreateScriptBuilder(null);

            IDbInspector dbInspector = utilityFactory.CreateDbInspector(connection, null);

            IDbTableInspector tableInspector = utilityFactory.CreateTableInspector(connection, null, "language");

            IDbCruder cruder = utilityFactory.CreateCruder(connection, null);

            IDbSerializer dbSerializer = utilityFactory.CreateDbSerializer(connection, null);

            // Assert
            Assert.That(dialect.Name, Is.EqualTo("PostgreSQL"));
            Assert.That(connection, Is.TypeOf<NpgsqlConnection>());
            Assert.That(dialect, Is.SameAs(NpgsqlDialect.Instance));

            Assert.That(scriptBuilder, Is.TypeOf<NpgsqlScriptBuilder>());
            Assert.That(scriptBuilder.CurrentOpeningIdentifierDelimiter, Is.EqualTo('"'));

            Assert.That(dbInspector, Is.TypeOf<NpgsqlInspector>());
            Assert.That(tableInspector, Is.TypeOf<NpgsqlTableInspector>());
            Assert.That(cruder, Is.TypeOf<NpgsqlCruder>());
            Assert.That(dbSerializer, Is.TypeOf<NpgsqlSerializer>());
        }
    }
}
