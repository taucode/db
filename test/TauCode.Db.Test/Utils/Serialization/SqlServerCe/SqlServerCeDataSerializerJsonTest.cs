using NUnit.Framework;
using TauCode.Db.Utils.Serialization;
using TauCode.Db.Utils.Serialization.SqlServerCe;
using TauCode.Utils.Extensions;

namespace TauCode.Db.Test.Utils.Serialization.SqlServerCe
{
    [TestFixture]
    public class SqlServerCeDataSerializerJsonTest : SqlServerCeTestBase
    {
        protected override string ResourceName => "rho-create-db.sql";

        [Test]
        public void DeserializeDb_ValidArguments_Deserializes()
        {
            // Arrange
            IDataSerializer dataSerializer = new SqlServerCeDataSerializer();
            var json = this.GetType().Assembly.GetResourceText("rho-data.json", true);

            // Act
            dataSerializer.DeserializeDb(this.Connection, json);

            // Assert
        }
    }
}
