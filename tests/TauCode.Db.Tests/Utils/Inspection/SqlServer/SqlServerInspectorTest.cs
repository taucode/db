using NUnit.Framework;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.SqlServer;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Tests.Utils.Inspection.SqlServer
{
    [TestFixture]
    public class SqlServerInspectorTest
    {
        private const string CONNECTION_STRING = @"Server=.\mssqltest;Database=taucode.db.test;User Id=testadmin;Password=1234;";

        private IDbConnection _connection;

        [SetUp]
        public void SetUp()
        {
            _connection = new SqlConnection(CONNECTION_STRING);
            _connection.Open();
        }

        [TearDown]
        public void TearDown()
        {
            _connection.Dispose();
            _connection = null;
        }

        [Test]
        public void GetTableInspector_ValidArgument_ReturnsTableInspector()
        {
            // Arrange

            IDbInspector dbInspector = new SqlServerInspector(_connection);
            dbInspector.PurgeDb();

            var entireSql = this.GetType().Assembly.GetResourceText("sqlserver-create-db.sql", true);
            var sqls = ScriptBuilderBase.SplitSqlByComments(entireSql);
            foreach (var sql in sqls)
            {
                dbInspector.ExecuteSql(sql);
            }

            // Act
            var tables = dbInspector.GetTableNames();
            var tableInspector = dbInspector.GetTableInspector("fragment_sub_type");

            // Assert
            var indexes = tableInspector.GetIndexMolds();
            var index = indexes.Single(x => x.Name == "UX_fragmentSubType_typeId_code");

            Assert.That(index.Columns, Has.Count.EqualTo(2));

            var indexColumn = index.Columns[0];
            Assert.That(indexColumn.Name, Is.EqualTo("type_id"));
            Assert.That(indexColumn.SortDirection, Is.EqualTo(SortDirection.Descending));

            indexColumn = index.Columns[1];
            Assert.That(indexColumn.Name, Is.EqualTo("code"));
            Assert.That(indexColumn.SortDirection, Is.EqualTo(SortDirection.Ascending));
        }
    }
}
