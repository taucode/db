using NUnit.Framework;
using TauCode.Db.SQLite;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Tests.SQLite
{
    [TestFixture]
    public class SQLiteSerializerPerformanceTests : TestBase
    {
        private IDbSerializer _dbSerializer;

        [SetUp]
        public void SetUp()
        {
            _dbSerializer = new SQLiteSerializer(this.Connection);
        }

        [Test]
        public void TodoWat()
        {
            // todo: add pragma!
            // todo: and remove unused jsons
            Assert.Pass("well...");
        }

        protected override void ExecuteDbCreationScript()
        {
            var migrator = new SQLiteJsonMigrator(
                this.Connection,
                () => this.GetType().Assembly.GetResourceText("performance.metadata.json", true),
                () => this.GetType().Assembly.GetResourceText("performance.data.json", true));
            migrator.Migrate();
        }
    }
}
