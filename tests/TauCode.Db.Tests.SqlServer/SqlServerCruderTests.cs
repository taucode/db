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
    }
}
