using NUnit.Framework;
using System.Data;
using System.Data.SqlClient;
using TauCode.Db.SqlServer;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Tests.SqlServer
{
    [TestFixture]
    public abstract class TestBase
    {
        protected IDbInspector DbInspector;
        protected IDbConnection Connection;

        protected const string ConnectionString = @"Server=.\mssqltest;Database=rho.test;User Id=testadmin;Password=1234;";

        [OneTimeSetUp]
        public void OneTimeSetUpBase()
        {
            Connection = new SqlConnection(ConnectionString);
            Connection.Open();

            DbInspector = new SqlServerInspector(Connection);
        }

        [OneTimeTearDown]
        public void OneTimeTearDownBase()
        {
            Connection.Dispose();
        }

        [SetUp]
        public void SetUpBase()
        {
            this.DropTables(); // todo: don't recreate all tables for each test, just purge the data.
            this.CreateTables();
        }

        protected void CreateTables()
        {
            var script = this.GetType().Assembly.GetResourceText("create-tables.sql", true);
            var sqls = DbUtils.SplitScriptByComments(script);

            foreach (var sql in sqls)
            {
                DbUtils.ExecuteSql(Connection, sql);
            }
        }

        protected void DropTables()
        {
            var script = this.GetType().Assembly.GetResourceText("drop-tables.sql", true);
            var sqls = DbUtils.SplitScriptByComments(script);

            foreach (var sql in sqls)
            {
                try
                {
                    DbUtils.ExecuteSql(Connection, sql);
                }
                catch
                {
                    // ignore exception - maybe table does not exist yet.
                }
            }
        }
    }
}
