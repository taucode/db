using NUnit.Framework;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using TauCode.Db.SQLite;

namespace TauCode.Db.Tests.SQLite
{
    [TestFixture]
    public abstract class TestBase
    {
        protected IDbInspector DbInspector;
        protected IDbConnection Connection;

        protected string TempDbFilePath;

        protected virtual void OneTimeSetUpImpl()
        {
            var tuple = TestHelper.CreateSQLiteConnectionString();
            this.TempDbFilePath = tuple.Item1;
            var connectionString = tuple.Item2;

            this.Connection = new SQLiteConnection(connectionString);
            this.Connection.Open();
            this.DbInspector = new SQLiteInspector(Connection);

            this.ExecuteDbCreationScript();
        }

        protected abstract void ExecuteDbCreationScript();

        protected virtual void OneTimeTearDownImpl()
        {
            this.Connection.Dispose();
            try
            {
                File.Delete(TempDbFilePath);
            }
            catch
            {
                // dismiss
            }
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
