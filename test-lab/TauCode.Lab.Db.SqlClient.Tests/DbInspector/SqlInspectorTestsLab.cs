using NUnit.Framework;
using TauCode.Db;

namespace TauCode.Lab.Db.SqlClient.Tests.DbInspector
{
    [TestFixture]
    public class SqlInspectorTestsLab : TestBase
    {
        #region Constructor

        /// <summary>
        /// Creates SqlInspector with valid connection and existing schema
        /// </summary>
        [Test]
        public void Constructor_ValidArguments_RunsOk()
        {
            // Arrange

            
            // Act
            IDbInspector inspector = new SqlInspectorLab(this.Connection, "dbo");

            // Assert
            Assert.That(inspector.Connection, Is.SameAs(this.Connection));
            Assert.That(inspector.Factory, Is.SameAs(SqlUtilityFactoryLab.Instance));

            Assert.That(inspector.SchemaName, Is.EqualTo("dbo"));
        }

        #endregion
    }
}
