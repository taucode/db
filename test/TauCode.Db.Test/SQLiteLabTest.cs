using NUnit.Framework;
using System.Linq;
using TauCode.Db.Utils.Crud.SQLite;

namespace TauCode.Db.Test
{
    [TestFixture]
    public class SQLiteLabTest : SQLiteTestBase
    {
        [Test]
        public void TableCreated_StandardTypes_PredictableColumnTypes()
        {
            // Arrange
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText =
@"
INSERT INTO [user](
    [id],
    [login],
    [age],
    [hash],
    [amount]
)
VALUES(
    @p_id,
    @p_login,
    @p_age,
    @p_hash,
    @p_amount
)
";

                command.AddParameterWithValue("p_id", 10);
                command.AddParameterWithValue("p_login", "wat");
                command.AddParameterWithValue("p_age", 10.3m);
                command.AddParameterWithValue("p_hash", new byte[] { 0x14, 0x88 });
                command.AddParameterWithValue("p_amount", 740.1488);

                command.ExecuteNonQuery();
            }

            // Act & Assert
            using (var command = this.Connection.CreateCommand())
            {
                command.CommandText =
@"
SELECT
    U.[id],
    U.[login],
    U.[age],
    U.[hash],
    U.[amount]
FROM
    [user] U
";
                var cruder = new SQLiteCruder();
                var row = cruder.GetRows(command).Single();

                Assert.That(row.id, Is.TypeOf<long>());
                Assert.That(row.login, Is.TypeOf<string>());
                Assert.That(row.age, Is.TypeOf<decimal>());
                Assert.That(row.hash, Is.TypeOf<byte[]>());
                Assert.That(row.amount, Is.TypeOf<double>());
            }
        }

        protected override string ResourceName => "sqlite-create-db-lab.sql";
    }
}
