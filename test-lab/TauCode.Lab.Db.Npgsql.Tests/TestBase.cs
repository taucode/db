using Npgsql;
using NUnit.Framework;

namespace TauCode.Lab.Db.Npgsql.Tests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected NpgsqlConnection Connection { get; private set; }

        [OneTimeSetUp]
        public void OneTimeSetUpBase()
        {
            this.Connection = TestHelper.CreateConnection();
        }

        [OneTimeTearDown]
        public void OneTimeTearDownBase()
        {
            this.Connection.Dispose();
            this.Connection = null;
        }

        [SetUp]
        public void SetUpBase()
        {
            this.Connection.PurgeDatabase();
        }
    }
}
