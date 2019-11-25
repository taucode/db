using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using TauCode.Db.Model;
using TauCode.Db.Utils.Building;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.MySql;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Tests.MySql.Utils.Inspection
{
    [TestFixture]
    public class MySqlInspectorTest
    {
        private const string CONNECTION_STRING = @"Server=localhost;Database=taucode.db.test;Uid=root;Pwd=1234;";

        private IDbConnection _connection;

        [SetUp]
        public void SetUp()
        {
            _connection = new MySqlConnection(CONNECTION_STRING);
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

            IDbInspector dbInspector = new MySqlInspector(_connection);
            dbInspector.PurgeDb();

            var entireSql = this.GetType().Assembly.GetResourceText("mysql-create-db.sql", true);
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
