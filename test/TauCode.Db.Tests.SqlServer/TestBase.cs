using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TauCode.Db.SqlServer;

namespace TauCode.Db.Tests.SqlServer
{
    [TestFixture]
    public abstract class TestBase
    {
        protected IDbInspector DbInspector;
        protected IDbConnection Connection;

        protected virtual void OneTimeSetUpImpl()
        {
            this.Connection = new SqlConnection(TestHelper.ConnectionString);
            this.Connection.Open();
            this.DbInspector = new SqlServerInspector(Connection);

            this.DbInspector.DropAllTables();
            this.ExecuteDbCreationScript();
        }

        protected abstract void ExecuteDbCreationScript();

        protected virtual void OneTimeTearDownImpl()
        {
            this.Connection.Dispose();
        }

        protected virtual void SetUpImpl()
        {
            this.DbInspector.DeleteDataFromAllTables();
        }

        protected virtual void TearDownImpl()
        {
        }

        [OneTimeSetUp]
        public void OneTimeSetUpBase()
        {
            this.OneTimeSetUpImpl();
        }

        [OneTimeTearDown]
        public void OneTimeTearDownBase()
        {
            this.OneTimeTearDownImpl();
        }

        [SetUp]
        public void SetUpBase()
        {
            this.SetUpImpl();
        }

        [TearDown]
        public void TearDownBase()
        {
            this.TearDownImpl();
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
