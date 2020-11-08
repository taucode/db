using MySql.Data.MySqlClient;
using NUnit.Framework;

// todo: get rid of 'dbo' in this project

namespace TauCode.Lab.Db.MySql.Tests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected MySqlConnection Connection { get; set; }

        [SetUp]
        public void SetUpBase()
        {
            this.Connection = TestHelper.CreateConnection(TestHelper.ConnectionString);
            this.Connection.Purge();
        }

        [TearDown]
        public void TearDownBase()
        {
            this.Connection.Dispose();
            this.Connection = null;
        }
    }
}
