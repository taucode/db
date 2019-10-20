using NUnit.Framework;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using TauCode.Utils.Extensions;

namespace TauCode.Db.XenoTest
{
    [TestFixture]
    public class WtfTest : DbTestBase
    {
        public const string DefaultConnectionString = @"Server=.\mssqltest;Database=econera.directory.test;User Id=testadmin;Password=1234;";

        [SetUp]
        public void SetUp()
        {
            this.ScriptBuilder.CurrentOpeningIdentifierDelimiter = '[';
            this.Cruder.ScriptBuilder.CurrentOpeningIdentifierDelimiter = '[';
        }


        [Test]
        public void DoTest()
        {
            // Arrange
            this.PurgeDb();
            this.Migrate();

            var json = this.GetType().Assembly.GetResourceText("testdb.json", true);
            this.DeserializeDbJson(json);
            
            // Act
            

            // Assert
        }

        protected override Assembly GetMigrationAssembly()
        {
            return typeof(M0_Baseline).Assembly;
        }

        protected override TargetDbType GetTargetDbType()
        {
            return TargetDbType.SqlServer;
        }

        protected override string GetConnectionString()
        {
            return DefaultConnectionString;
        }

        protected override IDbConnection CreateDbConnection(string connectionString)
        {
            return new SqlConnection(connectionString);
        }
    }
}
