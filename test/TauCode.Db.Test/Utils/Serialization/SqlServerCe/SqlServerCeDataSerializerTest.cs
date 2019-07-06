using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db.Data;
using TauCode.Db.Utils.Crud;
using TauCode.Db.Utils.Crud.SqlServerCe;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.SqlServerCe;
using TauCode.Db.Utils.Serialization;
using TauCode.Db.Utils.Serialization.SqlServerCe;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Test.Utils.Serialization.SqlServerCe
{
    [TestFixture]
    public class SqlServerCeDataSerializerTest : SqlServerCeTestBase
    {
        protected override string ResourceName => "sqlce-serialize-schema.sql";

        protected override string[] GetSqls()
        {
            var sqls = new List<string>();
            sqls.AddRange(base.GetSqls());

            var dataSqls = this.GetType().Assembly
                .GetResourceLines("sqlce-serialize-data.sql", true)
                .Where(x => !string.IsNullOrWhiteSpace(x));

            sqls.AddRange(dataSqls);
            return sqls.ToArray();
        }

        [Test]
        public void SerializeCommandResult_ValidArguments_Serializes()
        {
            // Arrange
            IDataSerializer dataSerializer = new SqlServerCeDataSerializer();

            // Act
            string json;
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM [user]";

                json = dataSerializer.SerializeCommandResult(command);
            }

            // Assert
            var expectedJson = this.GetType().Assembly.GetResourceText("sqlce-serialize-expected1.json", true);
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [Test]
        public void SerializeTable_ValidArguments_Serializes()
        {
            // Arrange
            IDataSerializer dataSerializer = new SqlServerCeDataSerializer();

            // Act
            string json;
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM [user]";

                json = dataSerializer.SerializeTable(this.Connection, "user");
            }

            // Assert
            var expectedJson = this.GetType().Assembly.GetResourceText("sqlce-serialize-expected1.json", true);
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [Test]
        public void SerializeDb_ValidArguments_Serializes()
        {
            // Arrange
            IDataSerializer dataSerializer = new SqlServerCeDataSerializer();

            // Act
            string json;
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText = @"SELECT * FROM [user]";

                json = dataSerializer.SerializeDb(this.Connection);
            }

            // Assert
            var expectedJson = this.GetType().Assembly.GetResourceText("sqlce-serialize-expected2.json", true);
            Assert.That(json, Is.EqualTo(expectedJson));
        }

        [Test]
        public void DeserializeTable_ValidArguments_Deserializes()
        {
            // Arrange
            this.ClearTablesData();
            IDataSerializer dataSerializer = new SqlServerCeDataSerializer();
            var json = this.GetType().Assembly.GetResourceText("sqlce-serialize-expected1.json", true);

            // Act
            var log = new Dictionary<int, object>();
            dataSerializer.RowDeserialized += (tableName, index, r) =>
            {
                var dynR = (DynamicRow)r;
                var names = dynR.GetNames();

                foreach (var name in names)
                {
                    if (dynR.GetValue(name) == null)
                    {
                        dynR.SetValue(name, DBNull.Value);
                    }
                }

                log[index] = r;
            };
            dataSerializer.DeserializeTable(this.Connection, "user", json);

            // Assert
            var cruder = new SqlServerCeCruder();
            var rows = cruder.GetRows(this.Connection, "user");

            var row = rows[0];
            Assert.That(((DynamicRow)row).IsEquivalentTo(log[0]), Is.True);
            Assert.That(row.id, Is.EqualTo(1));
            Assert.That(row.login, Is.EqualTo("ak"));
            Assert.That(row.birthday, Is.EqualTo(DateTime.Parse("1978-07-05T14:11:12.370")));
            Assert.That(row.email, Is.EqualTo("ak@mail.net"));
            Assert.That(row.password_hash, Is.EqualTo("1234"));

            row = rows[1];
            Assert.That(((DynamicRow)row).IsEquivalentTo(log[1]), Is.True);
            Assert.That(row.id, Is.EqualTo(2));
            Assert.That(row.login, Is.EqualTo("deserea"));
            Assert.That(row.birthday, Is.EqualTo(DBNull.Value));
            Assert.That(row.email, Is.EqualTo("deserea@love.org"));
            Assert.That(row.password_hash, Is.EqualTo(DBNull.Value));

            row = rows[2];
            Assert.That(((DynamicRow)row).IsEquivalentTo(log[2]), Is.True);
            Assert.That(row.id, Is.EqualTo(3));
            Assert.That(row.login, Is.EqualTo("olia"));
            Assert.That(row.birthday, Is.EqualTo(DateTime.Parse("1979-06-25T11:22:33.777")));
            Assert.That(row.email, Is.EqualTo(DBNull.Value));
            Assert.That(row.password_hash, Is.EqualTo("o@yahoo.com"));
        }

        [Test]
        public void DeserializeDb_ValidArguments_Deserializes()
        {
            // Arrange
            this.ClearTablesData();
            IDataSerializer dataSerializer = new SqlServerCeDataSerializer();
            var json = this.GetType().Assembly.GetResourceText("sqlce-serialize-expected2.json", true);

            // Act
            var log = new Dictionary<string, Dictionary<int, object>>();
            dataSerializer.RowDeserialized += (tableName, index, r) =>
            {
                var dynR = (DynamicRow)r;
                var names = dynR.GetNames();

                foreach (var name in names)
                {
                    if (dynR.GetValue(name) == null)
                    {
                        dynR.SetValue(name, DBNull.Value);
                    }
                }

                if (!log.ContainsKey(tableName))
                {
                    log.Add(tableName, new Dictionary<int, object>());
                }

                var table = log[tableName];
                table[index] = r;
            };

            dataSerializer.DeserializeDb(this.Connection, json);

            // Assert
            var cruder = new SqlServerCeCruder();

            // table: [user]
            var rows = cruder.GetRows(this.Connection, "user");
            Assert.That(rows, Has.Count.EqualTo(3));

            var row = rows[0];
            Assert.That(((DynamicRow)row).IsEquivalentTo(log["user"][0]), Is.True);
            Assert.That(row.id, Is.EqualTo(1));
            Assert.That(row.login, Is.EqualTo("ak"));
            Assert.That(row.birthday, Is.EqualTo(DateTime.Parse("1978-07-05T14:11:12.370")));
            Assert.That(row.email, Is.EqualTo("ak@mail.net"));
            Assert.That(row.password_hash, Is.EqualTo("1234"));

            row = rows[1];
            Assert.That(((DynamicRow)row).IsEquivalentTo(log["user"][1]), Is.True);
            Assert.That(row.id, Is.EqualTo(2));
            Assert.That(row.login, Is.EqualTo("deserea"));
            Assert.That(row.birthday, Is.EqualTo(DBNull.Value));
            Assert.That(row.email, Is.EqualTo("deserea@love.org"));
            Assert.That(row.password_hash, Is.EqualTo(DBNull.Value));

            row = rows[2];
            Assert.That(((DynamicRow)row).IsEquivalentTo(log["user"][2]), Is.True);
            Assert.That(row.id, Is.EqualTo(3));
            Assert.That(row.login, Is.EqualTo("olia"));
            Assert.That(row.birthday, Is.EqualTo(DateTime.Parse("1979-06-25T11:22:33.777")));
            Assert.That(row.email, Is.EqualTo(DBNull.Value));
            Assert.That(row.password_hash, Is.EqualTo("o@yahoo.com"));

            // table: [item]
            rows = cruder.GetRows(this.Connection, "item");
            Assert.That(rows, Has.Count.EqualTo(2));

            row = rows[0];
            Assert.That(((DynamicRow)row).IsEquivalentTo(log["item"][0]), Is.True);
            Assert.That(row.id, Is.EqualTo(1));
            Assert.That(row.user_id, Is.EqualTo(1));
            Assert.That(row.name, Is.EqualTo("ud"));
            Assert.That(row.description, Is.EqualTo("19.5"));
            Assert.That(row.is_active, Is.EqualTo(true));

            row = rows[1];
            Assert.That(((DynamicRow)row).IsEquivalentTo(log["item"][1]), Is.True);
            Assert.That(row.id, Is.EqualTo(2));
            Assert.That(row.user_id, Is.EqualTo(2));
            Assert.That(row.name, Is.EqualTo("p"));
            Assert.That(row.description, Is.EqualTo("sweet"));
            Assert.That(row.is_active, Is.EqualTo(true));
        }

        private void ClearTablesData()
        {
            var dbInspector = new SqlServerCeInspector(this.Connection);
            dbInspector.ClearDb();
        }
    }
}
