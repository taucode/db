using NUnit.Framework;
using System;
using System.Linq;
using System.Text;
using TauCode.Db.SqlServer;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Tests.SqlServer
{
    [TestFixture]
    public class SqlServerSerializerComplexTests : TestBase
    {
        private IDbSerializer _dbSerializer;

        [SetUp]
        public void SetUp()
        {
            this.DbInspector.DropAllTables();

            var ddlScript = TestHelper.GetResourceText("ocean.script-create-tables.sql");
            this.Connection.ExecuteCommentedScript(ddlScript);

            _dbSerializer = new SqlServerSerializer(this.Connection);
            _dbSerializer.ScriptBuilder.CurrentOpeningIdentifierDelimiter = '[';
        }

        [Test]
        public void SerializeDbData_ComplexData_ProducesExpectedResult()
        {
            // Arrange
            var insertScript = TestHelper.GetResourceText("ocean.script-data.sql");
            this.Connection.ExecuteCommentedScript(insertScript);

            // Act
            var json = _dbSerializer.SerializeDbData();

            // Assert
            var expectedJson = this.GetType().Assembly.GetResourceText("ocean.data-db.json", true);

            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [Test]
        public void DeserializeDbData_ComplexData_ProducesExpectedResult()
        {
            // Arrange
            var json = this.GetType().Assembly.GetResourceText("ocean.data-db.json", true);

            // Act
            _dbSerializer.DeserializeDbData(json);

            // Assert
            var cruder = this.DbInspector.Factory.CreateCruder(this.Connection);
            cruder.ScriptBuilder.CurrentOpeningIdentifierDelimiter = '[';
            var users = cruder.GetAllRows("user");
            var userInfos = cruder.GetAllRows("user_info");

            // ak
            var id = new Guid("115777dc-2394-4e14-a587-11afde55588e");
            var user = users.Single(x => x.id == id);
            Assert.That(user.name, Is.EqualTo("ak"));
            Assert.That(user.birthday, Is.EqualTo(new DateTime(1978, 7, 5, 11, 20, 30)));
            Assert.That(user.gender, Is.False);
            CollectionAssert.AreEqual(GetResourceBytes("ak.png"), user.picture);

            var userInfo = userInfos.Single(x => x.user_id == id);
            Assert.That(userInfo.id, Is.EqualTo(new Guid("118833be-1ac7-4161-90d5-11eaa22d1609")));
            Assert.That(userInfo.tax_number, Is.EqualTo("TXNM-111  "));
            Assert.That(userInfo.code, Is.EqualTo("COD-Андрей          "));
            Assert.That(userInfo.ansi_name, Is.EqualTo("ANSI NAME Andrey"));
            Assert.That(userInfo.ansi_description, Is.EqualTo("ANSI DESCR Andrey Kovalenko"));
            Assert.That(userInfo.unicode_description, Is.EqualTo("UNICODE Андрей Коваленко"));
            Assert.That(userInfo.height, Is.EqualTo(1.79));
            Assert.That(userInfo.weight, Is.EqualTo(68.9));
            Assert.That(userInfo.weight2, Is.EqualTo(92.1));
            Assert.That(userInfo.salary, Is.EqualTo(6000.3m));
            Assert.That(userInfo.rating_decimal, Is.EqualTo(19.5m));
            Assert.That(userInfo.rating_numeric, Is.EqualTo(7.67717m));
            Assert.That(userInfo.num8, Is.EqualTo(14));
            Assert.That(userInfo.num16, Is.EqualTo(1488));
            Assert.That(userInfo.num32, Is.EqualTo(17401488));
            Assert.That(userInfo.num64, Is.EqualTo(188887401488));

            throw new NotImplementedException("go on!");
        }

        private byte[] GetResourceBytes(string fileName)
        {
            return this.GetType().Assembly.GetResourceBytes(fileName, true);
        }


        [Test]
        public void WatTodo()
        {
            //var dir = @"C:\work\tau\lib\taucode.db\tests\TauCode.Db.Tests.SqlServer\Resources\ocean\assets\";
            //var dirInfo = new DirectoryInfo(dir);
            //var files = dirInfo.GetFiles();

            //foreach (var file in files)
            //{
            //    var bytes = File.ReadAllBytes(file.FullName);
            //    var sb = new StringBuilder("0x");
            //    this.WriteHex(sb, bytes);
            //    var txt = sb.ToString();

            //    var fnameWithExtension = file.Name;
            //    var fname = Path.GetFileNameWithoutExtension(fnameWithExtension);
            //    fname += ".txt";
            //    var fpath = Path.Combine(dirInfo.FullName, fname);
            //    File.WriteAllText(fpath, txt);
            //}
        }

        // todo temp
        private void WriteHex(StringBuilder sb, byte[] bytes)
        {
            foreach (var b in bytes)
            {
                sb.AppendFormat("{0:x2}", b);
            }
        }

        protected override void ExecuteDbCreationScript()
        {
            var script = TestHelper.GetResourceText("rho.script-create-tables.sql");
            this.Connection.ExecuteCommentedScript(script);
        }
    }
}
