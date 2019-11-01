using NUnit.Framework;
using System;
using System.Data.SqlClient;
using System.Linq;
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

        [Test]
        public void Wat()
        {
            // Arrange
            var con = new SqlConnection(CONNECTION_STRING);
            con.Open();

            var dbInspector = new SqlServerInspector(con);
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

            throw new NotImplementedException();

            con.Dispose();
        }
    }
}
