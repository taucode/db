using NUnit.Framework;
using System;
using System.Linq;

namespace TauCode.Db.Tests.SqlServer
{
    [TestFixture]
    public class SqlServerCruderTests : TestBase
    {
        private ICruder _cruder;

        [SetUp]
        public void SetUp()
        {
            _cruder = this.DbInspector.Factory.CreateCruder(this.Connection);
            _cruder.ScriptBuilderLab.CurrentOpeningIdentifierDelimiter = '[';
        }

        [Test]
        public void InsertRow_ValidRow_Inserts()
        {
            // Arrange
            var id = new Guid("bf7f7507-4b08-48f0-9bbe-d061acdbca31");

            var language = new
            {
                id,
                code = "it",
                name = "Italian",
            };

            // Act
            _cruder.InsertRow("language", language);

            // Assert
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText = @"SELECT [id], [code], [name] FROM [language] WHERE [id] = @p_id";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "p_id";
                parameter.Value = id;
                command.Parameters.Add(parameter);
                var row = DbUtils.GetCommandRows(command).Single();

                Assert.That(row.id, Is.EqualTo(id));
                Assert.That(row.code, Is.EqualTo("it"));
                Assert.That(row.name, Is.EqualTo("Italian"));
            }
        }

        [Test]
        public void InsertRow_WrongColumnValue_ThrowsTauCodeDbException()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRow_NonExistingColumnNames_ThrowsTauCodeDbException()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void UpdateRow_ValidData_UpdatesAndReturnsTrue()
        {
            // Arrange
            var id = new Guid("bf7f7507-4b08-48f0-9bbe-d061acdbca31");

            var language = new
            {
                id,
                code = "it",
                name = "Italian",
            };

            _cruder.InsertRow("language", language);

            // Act
            var updated = _cruder.UpdateRow("language", new {language = "Duzhe Italian!"}, id);

            // Assert
            var row = this.GetRow(id);
            Assert.That(row.id, Is.EqualTo(id));
            Assert.That(row.code, Is.EqualTo("it"));
            Assert.That(row.name, Is.EqualTo("Duzhe Italian!"));
        }

        [Test]
        public void UpdateRow_NonExistingId_DoesNothingAndReturnsFalse()
        {
            // Arrange
            var id = new Guid("bf7f7507-4b08-48f0-9bbe-d061acdbca31");

            var language = new
            {
                id,
                code = "it",
                name = "Italian",
            };

            _cruder.InsertRow("language", language);

            // Act
            _cruder.UpdateRow("language", new { language = "Duzhe Italian!" }, id);

            // Assert
            var row = this.GetRow(id);
            Assert.That(row.id, Is.EqualTo(id));
            Assert.That(row.code, Is.EqualTo("it"));
            Assert.That(row.name, Is.EqualTo("Duzhe Italian!"));
        }

        [Test]
        public void UpdateRow_NonExistingColumnNames_ThrowsTauCodeDbException()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void UpdateRow_PrimaryKeyColumnIncluded_ThrowsTauCodeDbException()
        {
            throw new NotImplementedException();
        }

        [Test]
        public void UpdateRow_WrongColumnValue_ThrowsTauCodeDbException()
        {
            throw new NotImplementedException();
        }

        private dynamic GetRow(object id)
        {
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText = @"SELECT [id], [code], [name] FROM [language] WHERE [id] = @p_id";
                var parameter = command.CreateParameter();
                parameter.ParameterName = "p_id";
                parameter.Value = id;
                command.Parameters.Add(parameter);
                var row = DbUtils.GetCommandRows(command).Single();
                return row;
            }
        }
    }
}
