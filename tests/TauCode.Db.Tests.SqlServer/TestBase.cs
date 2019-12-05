using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TauCode.Db.SqlServer;
using TauCode.Db.Tests.Common;
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
            var script = CommonTestHelper.GetResourceText("rho.script-create-tables.sql");
            this.Connection.ExecuteCommentedScript(script);
        }

        protected void DropTables()
        {
            var script = CommonTestHelper.GetResourceText("rho.script-drop-tables.sql");
            this.Connection.ExecuteCommentedScript(script);
        }

        protected dynamic GetRow(string tableName, object id)
        {
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText = $@"SELECT * FROM [{tableName}] WHERE [id] = @p_id";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "p_id";
                parameter.Value = id;
                command.Parameters.Add(parameter);
                var row = DbUtils.GetCommandRows(command).SingleOrDefault();
                return row;
            }
        }

        protected IList<dynamic> GetRows(string tableName)
        {
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText = $@"SELECT * FROM [{tableName}]";
                var rows = DbUtils.GetCommandRows(command);
                return rows;
            }
        }
    }
}
