using MySql.Data.MySqlClient;
using NUnit.Framework;

namespace TauCode.Lab.Db.MySql.Tests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected MySqlConnection Connection { get; set; }

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
            this.Connection.Purge();
        }
    }
}
