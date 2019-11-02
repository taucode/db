using NUnit.Framework;
using System.Data;
using System.IO;
using TauCode.Db.Utils.Building;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Tests
{
    [TestFixture]
    public abstract class TestBase
    {
        protected string TempDbFilePath { get; set; }
        protected string ConnectionString { get; set; }
        protected IDbConnection Connection { get; set; }

        protected abstract string ResourceName { get; }
        protected abstract string CreateConnectionString();
        protected abstract IDbConnection CreateConnection();

        [SetUp]
        public void SetUpBase()
        {
            this.CreateDatabase();
        }

        [TearDown]
        public void TearDownBase()
        {
            this.DeleteDatabaseSafely();
        }

        protected virtual string[] GetSqls()
        {
            var sql = this.GetType().Assembly.GetResourceText(this.ResourceName, true);
            var statements = ScriptBuilderBase.SplitSqlByComments(sql);
            return statements;
        }

        protected void CreateDatabase()
        {
            this.DeleteDatabaseSafely();

            this.TempDbFilePath = FileExtensions.CreateTempFilePath("ztemp", ".nunit-db");
            this.ConnectionString = this.CreateConnectionString();
            this.Connection = this.CreateConnection();

            this.Connection.Open();

            var statements = this.GetSqls();
            foreach (var statement in statements)
            {
                using (var command = this.Connection.CreateCommand())
                {
                    command.CommandText = statement;
                    command.ExecuteNonQuery();
                }
            }
        }

        protected void DeleteDatabaseSafely()
        {
            if (this.Connection != null)
            {
                try
                {
                    this.Connection.Dispose();
                }
                catch
                {
                    // dismiss
                }
            }

            if (this.TempDbFilePath != null)
            {
                try
                {
                    File.Delete(this.TempDbFilePath);
                }
                catch
                {
                    // dismiss
                }
            }

            this.Connection = null;
            this.TempDbFilePath = null;
            this.ConnectionString = null;
        }
    }
}
