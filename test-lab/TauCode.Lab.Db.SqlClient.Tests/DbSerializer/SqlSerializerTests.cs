using Microsoft.Data.SqlClient;
using NUnit.Framework;
using System;
using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient.Tests.DbSerializer
{
    [TestFixture]
    public class SqlSerializerTests : TestBase
    {
        #region Constructor

        [Test]
        [TestCase("dbo")]
        [TestCase(null)]
        public void Constructor_ValidArguments_RunsOk(string schemaName)
        {
            // Arrange

            // Act
            IDbSerializer serializer = new SqlSerializerLab(this.Connection, schemaName);

            // Assert
            Assert.That(serializer.Connection, Is.SameAs(this.Connection));
            Assert.That(serializer.Factory, Is.SameAs(SqlUtilityFactoryLab.Instance));
            Assert.That(serializer.SchemaName, Is.EqualTo("dbo"));
            Assert.That(serializer.Cruder, Is.TypeOf<SqlCruderLab>());
        }

        [Test]
        public void Constructor_ConnectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new SqlSerializerLab(null, "dbo"));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
        }

        [Test]
        public void Constructor_ConnectionIsNotOpen_ArgumentException()
        {
            // Arrange
            using var connection = new SqlConnection(TestHelper.ConnectionString);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => new SqlSerializerLab(connection, "dbo"));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
            Assert.That(ex.Message, Does.StartWith("Connection should be opened."));
        }


        #endregion

        #region SerializeTableData

        [Test]
        public void SerializeTableData_ValidArguments_RunsOk()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SerializeTableData_TableNameIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SerializeTableData_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SerializeTableData_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region SerializeDbData

        [Test]
        public void SerializeDbData_ValidArguments_RunsOk()
        {
            // Arrange

            // Act
            // todo: - respects 'tableNamePredicate'

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SerializeDbData_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SerializeDbData_TableNamePredicateIsNull_SerializesDbData()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SerializeDbData_TableNamePredicateIsFalser_ReturnsEmptyArray()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region DeserializeTableData

        [Test]
        public void DeserializeTableData_ValidArguments_RunsOk()
        {
            // Arrange

            // Act
            // todo - respects rowTransformer

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeserializeTableData_TableNameIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeserializeTableData_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeserializeTableData_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeserializeTableData_JsonIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeserializeTableData_JsonContainsBadData_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region DeserializeDbData

        [Test]
        public void DeserializeDbData_ValidArguments_RunsOk()
        {
            // Arrange

            // Act
            // todo - respects table predicate
            // todo - respects rowTransformer

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeserializeDbData_JsonIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeserializeDbData_TablePredicateIsNull_DeserializesAll()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeserializeDbData_TablePredicateReturnsUnknownTable_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeserializeDbData_JsonContainsBadData_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeserializeDbData_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region SerializeTableMetadata

        [Test]
        public void SerializeTableMetadata_ValidArguments_RunsOk()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SerializeTableMetadata_TableNameIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SerializeTableMetadata_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SerializeTableMetadata_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region SerializeDbMetadata

        [Test]
        public void SerializeDbMetadata_ValidArguments_RunsOk()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void SerializeDbMetadata_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        #endregion
    }
}
