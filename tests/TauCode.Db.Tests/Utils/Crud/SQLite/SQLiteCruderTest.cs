using NUnit.Framework;
using System.Linq;
using TauCode.Db.Utils.Crud;
using TauCode.Db.Utils.Crud.SQLite;

namespace TauCode.Db.Tests.Utils.Crud.SQLite
{
    [TestFixture]
    public class SQLiteCruderTest : SQLiteTestBase
    {
        [Test]
        public void InsertRow_RowWithOmitedOptionalColumnValues_Inserts()
        {
            // Arrange
            ICruder cruder = new SQLiteCruder(this.Connection);

            var row = new
            {
                id = 1,
                login = "ak",
            };

            // Act
            cruder.InsertRow("user", row);

            // Assert

            // Normally, it is not a good idea to test some functionality with use of this very functionality.
            // But I am pretty sure ICruder.GetRows() works pretty fine since it is also covered by UTs.
            ICruder assertCruder = new SQLiteCruder(this.Connection);

            var rows = assertCruder.GetRows("user");

            Assert.That(rows, Has.Count.EqualTo(1));
            var assertRow = rows.Single();
            Assert.That(assertRow.id, Is.EqualTo(1));
            Assert.That(assertRow.login, Is.EqualTo("ak"));
            Assert.That(assertRow.email, Is.EqualTo("ak@deserea.net"));
            Assert.That(assertRow.password_hash, Is.EqualTo("nohash"));
        }

        [Test]
        public void GetRow_ExistingRowId_ReturnsRow()
        {
            // Arrange
            // Normally, it is not a good idea to test some functionality with use of this very functionality.
            // But I am pretty sure ICruder.InsertRow() works pretty fine since it is also covered by UTs.
            ICruder setupCruder = new SQLiteCruder(this.Connection);

            var row = new
            {
                id = 1,
                login = "ak",
            };

            setupCruder.InsertRow("user", row);

            // Act
            ICruder testCruder = new SQLiteCruder(this.Connection);
            var existingRow = testCruder.GetRow("user", 1);

            // Assert
            Assert.That(existingRow.id, Is.EqualTo(1));
            Assert.That(existingRow.login, Is.EqualTo("ak"));
            Assert.That(existingRow.email, Is.EqualTo("ak@deserea.net"));
            Assert.That(existingRow.password_hash, Is.EqualTo("nohash"));
        }

        [Test]
        public void GetRow_NonExistingRowId_ReturnsNull()
        {
            // Arrange
            // Normally, it is not a good idea to test some functionality with use of this very functionality.
            // But I am pretty sure ICruder.InsertRow() works pretty fine since it is also covered by UTs.
            var setupCruder = new SQLiteCruder(this.Connection);

            var row = new
            {
                id = 1,
                login = "ak",
            };

            setupCruder.InsertRow("user", row);

            // Act
            var testCruder = new SQLiteCruder(this.Connection);
            var nonExistingRow = testCruder.GetRow("user", 1488);

            // Assert
            Assert.That(nonExistingRow, Is.Null);
        }

        [Test]
        public void DeleteRow_ExistingId_DeletesRowAndReturnsTrue()
        {
            this.AddSetupRow(1, "ak");
            this.AddSetupRow(2, "olia");
            this.AddSetupRow(3, "ira");
            this.AddSetupRow(4, "marina");

            // Act
            ICruder testCruder = new SQLiteCruder(this.Connection);
            var deleted = testCruder.DeleteRow("user", 3);

            // Assert
            Assert.That(deleted, Is.True);
            ICruder assertCruder = new SQLiteCruder(this.Connection);
            var remaining = assertCruder.GetRows("user");
            var remainingIds = remaining.Select(x => (int)x.id);
            CollectionAssert.AreEquivalent(
                new[] { 1, 2, 4 },
                remainingIds);
        }

        [Test]
        public void DeleteRow_NonExistingId_DoesNotDeleteRowAndReturnsFalse()
        {
            this.AddSetupRow(1, "ak");
            this.AddSetupRow(2, "olia");
            this.AddSetupRow(3, "ira");
            this.AddSetupRow(4, "marina");

            // Act
            ICruder testCruder = new SQLiteCruder(this.Connection);
            var deleted = testCruder.DeleteRow("user", 1488);

            // Assert
            Assert.That(deleted, Is.False);
            ICruder assertCruder = new SQLiteCruder(this.Connection);
            var remaining = assertCruder.GetRows("user");
            var remainingIds = remaining.Select(x => (int)x.id);
            CollectionAssert.AreEquivalent(
                new[] { 1, 2, 3, 4 },
                remainingIds);
        }

        [Test]
        public void UpdateRow_ExistingId_UpdatesRowAndReturnsTrue()
        {
            this.AddSetupRow(1, "ak");
            this.AddSetupRow(2, "olia");
            this.AddSetupRow(3, "ira");
            this.AddSetupRow(4, "marina");

            // Act
            ICruder testCruder = new SQLiteCruder(this.Connection);
            var updated = testCruder.UpdateRow(
                "user",
                new
                {
                    login = "olga",
                    email = "night_miracle@yahoo.com",
                    password_hash = "love",
                },
                2);

            // Assert
            Assert.That(updated, Is.True);
            ICruder assertCruder = new SQLiteCruder(this.Connection);
            var updatedOlia = assertCruder.GetRow("user", 2);
            Assert.That(updatedOlia.login, Is.EqualTo("olga"));
            Assert.That(updatedOlia.email, Is.EqualTo("night_miracle@yahoo.com"));
            Assert.That(updatedOlia.password_hash, Is.EqualTo("love"));

            // other not changed
            var u1 = assertCruder.GetRow("user", 1);
            Assert.That(u1.login, Is.EqualTo("ak"));

            var u3 = assertCruder.GetRow("user", 3);
            Assert.That(u3.login, Is.EqualTo("ira"));

            var u4 = assertCruder.GetRow("user", 4);
            Assert.That(u4.login, Is.EqualTo("marina"));

            var users = new[] { u1, u3, u4 };
            foreach (var user in users)
            {
                Assert.That(user.email, Is.EqualTo("ak@deserea.net"));
                Assert.That(user.password_hash, Is.EqualTo("nohash"));
            }
        }

        [Test]
        public void UpdateRow_NonExistingId_DoesNotUpdateRowAndReturnsFalse()
        {
            this.AddSetupRow(1, "ak");
            this.AddSetupRow(2, "olia");
            this.AddSetupRow(3, "ira");
            this.AddSetupRow(4, "marina");

            // Act
            ICruder testCruder = new SQLiteCruder(this.Connection);
            var updated = testCruder.UpdateRow(
                "user",
                new
                {
                    login = "olga",
                    email = "night_miracle@yahoo.com",
                    password_hash = "love",
                },
                1488);

            // Assert
            Assert.That(updated, Is.False);
            ICruder assertCruder = new SQLiteCruder(this.Connection);

            // other not changed
            var u1 = assertCruder.GetRow("user", 1);
            Assert.That(u1.login, Is.EqualTo("ak"));

            var u2 = assertCruder.GetRow("user", 2);
            Assert.That(u2.login, Is.EqualTo("olia"));

            var u3 = assertCruder.GetRow("user", 3);
            Assert.That(u3.login, Is.EqualTo("ira"));

            var u4 = assertCruder.GetRow("user", 4);
            Assert.That(u4.login, Is.EqualTo("marina"));

            var users = new[] { u1, u3, u4 };
            foreach (var user in users)
            {
                Assert.That(user.email, Is.EqualTo("ak@deserea.net"));
                Assert.That(user.password_hash, Is.EqualTo("nohash"));
            }
        }

        protected override string ResourceName => "sqlite-cruder-test.sql";

        private void AddSetupRow(int id, string login)
        {
            ICruder cruder = new SQLiteCruder(this.Connection);

            var row = new
            {
                id,
                login,
            };

            // Act
            cruder.InsertRow("user", row);
        }
    }
}
