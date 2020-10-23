using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Data;
using TauCode.Db.DbValueConverters;
using TauCode.Db.Exceptions;
using TauCode.Extensions;

namespace TauCode.Lab.Db.SqlClient.Tests.DbCruder
{
    [TestFixture]
    public class SqlCruderTests : TestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Connection.CreateSchema("zeta");

            var sql = this.GetType().Assembly.GetResourceText("crebase.sql", true);
            this.Connection.ExecuteCommentedScript(sql);
        }

        private void TodoCompare(string actual, string expected, string extension = "sql")
        {
            TestHelper.WriteDiff(actual, expected, @"c:\temp\0-sql\", extension, "todo");
        }

        private void CreateSuperTable()
        {
            var sql = this.GetType().Assembly.GetResourceText("SuperTable.sql", true);
            this.Connection.ExecuteSingleSql(sql);
        }

        private void CreateMediumTable()
        {
            var sql = @"
CREATE TABLE [zeta].[MediumTable](
    [Id] int NOT NULL PRIMARY KEY,

    [TheInt] int NULL DEFAULT 1599,
    [TheNVarChar] nvarchar(100) NULL DEFAULT 'Semmi')
";

            this.Connection.ExecuteSingleSql(sql);
        }

        private void CreateSmallTable()
        {
            var sql = @"
CREATE TABLE [zeta].[SmallTable](
    [Id] int NOT NULL PRIMARY KEY IDENTITY(1, 1),

    [TheInt] int NULL DEFAULT 1599,
    [TheNVarChar] nvarchar(100) NULL DEFAULT 'Semmi')
";

            this.Connection.ExecuteSingleSql(sql);
        }

        #region Constructor

        [Test]
        [TestCase("dbo")]
        [TestCase(null)]
        public void Constructor_ValidArguments_RunsOk(string schemaName)
        {
            // Arrange

            // Act
            IDbCruder cruder = new SqlCruderLab(this.Connection, schemaName);

            // Assert
            Assert.That(cruder.Connection, Is.SameAs(this.Connection));
            Assert.That(cruder.Factory, Is.SameAs(SqlUtilityFactoryLab.Instance));
            Assert.That(cruder.SchemaName, Is.EqualTo("dbo"));
            Assert.That(cruder.ScriptBuilder, Is.TypeOf<SqlScriptBuilderLab>());
            Assert.That(cruder.RowInsertedCallback, Is.Null);
        }

        [Test]
        public void Constructor_ConnectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new SqlCruderLab(null, "dbo"));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
        }

        [Test]
        public void Constructor_ConnectionIsNotOpen_ArgumentException()
        {
            // Arrange
            using var connection = new SqlConnection(TestHelper.ConnectionString);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => new SqlCruderLab(connection, "dbo"));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
            Assert.That(ex.Message, Does.StartWith("Connection should be opened."));
        }

        #endregion

        #region GetTableValuesConverter

        [Test]
        public void GetTableValuesConverter_ValidArgument_ReturnsProperConverter()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var converter = cruder.GetTableValuesConverter("PersonData");

            // Assert
            var dbValueConverter = converter.GetColumnConverter("Id");
            Assert.That(dbValueConverter, Is.TypeOf<GuidValueConverter>());
        }

        [Test]
        public void GetTableValuesConverter_ArgumentIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.GetTableValuesConverter(null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void GetTableValuesConverter_NotExistingSchema_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetTableValuesConverter("some_table"));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetTableValuesConverter_NotExistingTable_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetTableValuesConverter("bad_table"));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        #endregion

        #region ResetTableValuesConverters

        [Test]
        public void ResetTableValuesConverters_NoArguments_RunsOk()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");
            cruder.GetTableValuesConverter("PersonData").SetColumnConverter("Id", new StringValueConverter());
            var oldDbValueConverter = cruder.GetTableValuesConverter("PersonData").GetColumnConverter("Id");

            // Act
            cruder.ResetTableValuesConverters();
            var resetDbValueConverter = cruder.GetTableValuesConverter("PersonData").GetColumnConverter("Id");

            // Assert
            Assert.That(oldDbValueConverter, Is.TypeOf<StringValueConverter>());
            Assert.That(resetDbValueConverter, Is.TypeOf<GuidValueConverter>());
        }

        #endregion

        #region InsertRow

        [Test]
        public void InsertRow_ValidArguments_InsertsRow()
        {
            // Arrange
            var row1 = new Dictionary<string, object>
            {
                {"Id", new Guid("a776fd76-f2a8-4e09-9e69-b6d08e96c075")},
                {"PersonId", 101},
                {"Weight", 69.20m},
                {"PersonMetaKey", (short) 12},
                {"IQ", 101.60m},
                {"Temper", (short) 4},
                {"PersonOrdNumber", (byte) 3},
                {"MetricB", -3},
                {"MetricA", 177},
                {"NotExisting", 11},
            };
            
            var row2 = new DynamicRow();
            row2.SetValue("Id", new Guid("a776fd76-f2a8-4e09-9e69-b6d08e96c075"));
            row2.SetValue("PersonId", 101);
            row2.SetValue("Weight", 69.20m);
            row2.SetValue("PersonMetaKey", (short)12);
            row2.SetValue("IQ", 101.60m);
            row2.SetValue("Temper", (short)4);
            row2.SetValue("PersonOrdNumber", (byte)3);
            row2.SetValue("MetricB", -3);
            row2.SetValue("MetricA", 177);
            row2.SetValue("NotExisting", 11);

            var row3 = new
            {
                Id = new Guid("a776fd76-f2a8-4e09-9e69-b6d08e96c075"),
                PersonId = 101,
                Weight = 69.20m,
                PersonMetaKey = (short)12,
                IQ = 101.60m,
                Temper = (short)4,
                PersonOrdNumber = (byte)3,
                MetricB = -3,
                MetricA = 177,
                NotExisting = 11,
            };

            var row4 = new HealthInfoDto
            {
                Id = new Guid("a776fd76-f2a8-4e09-9e69-b6d08e96c075"),
                PersonId = 101,
                Weight = 69.20m,
                PersonMetaKey = 12,
                IQ = 101.60m,
                Temper = 4,
                PersonOrdNumber = 3,
                MetricB = -3,
                MetricA = 177,
                NotExisting = 11,
            };

            object[] rows =
            {
                row1,
                row2,
                row3,
                row4,
            };

            IReadOnlyDictionary<string, object>[] loadedRows = new IReadOnlyDictionary<string, object>[rows.Length];

            this.Connection.ExecuteSingleSql("ALTER TABLE [zeta].[HealthInfo] DROP CONSTRAINT [FK_healthInfo_Person]");

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            for (var i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                cruder.InsertRow("HealthInfo", row, x => x != "NotExisting");
                var loadedRow = TestHelper.LoadRow(
                    this.Connection,
                    "zeta",
                    "HealthInfo",
                    new Guid("a776fd76-f2a8-4e09-9e69-b6d08e96c075"));

                loadedRows[i] = loadedRow;

                this.Connection.ExecuteSingleSql("DELETE FROM [zeta].[HealthInfo]");
            }

            // Assert
            for (var i = 0; i < loadedRows.Length; i++)
            {
                var originalRow = rows[i];
                var cleanOriginalRow = new DynamicRow(originalRow);
                cleanOriginalRow.DeleteValue("NotExisting");

                var originalRowJson = JsonConvert.SerializeObject(cleanOriginalRow);
                var loadedJson = JsonConvert.SerializeObject(loadedRows[i]);

                Assert.That(loadedJson, Is.EqualTo(originalRowJson));
            }
        }

        [Test]
        public void InsertRow_AllDataTypes_RunsOk()
        {
            // Arrange
            this.CreateSuperTable();

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            dynamic row = new DynamicRow(new
            {
                TheGuid = new Guid("8e816a5f-b97c-43df-95e9-4fbfe7172dd0"),

                TheBit = true,

                TheTinyInt = (byte)17,
                TheSmallInt = (short)11,
                TheInt = 44,
                TheBigInt = 777L,

                TheDecimal = 11.2m,
                TheNumeric = 22.3m,

                TheSmallMoney = 123.06m,
                TheMoney = 60.77m,

                TheReal = (float)15.99,
                TheFloat = 7001.555,

                TheDate = DateTime.Parse("2010-01-02"),
                TheDateTime = DateTime.Parse("2011-11-12T10:10:10"),
                TheDateTime2 = DateTime.Parse("2015-03-07T05:06:33.777"),
                TheDateTimeOffset = DateTimeOffset.Parse("2011-11-12T10:10:10+03:00"),
                TheSmallDateTime = DateTime.Parse("1970-04-08T11:11:11"),
                TheTime = TimeSpan.Parse("03:03:03"),

                TheChar = "abc",
                TheVarChar = "Andrey Kovalenko",
                TheVarCharMax = "Rocky Marciano",

                TheNChar = "АБВ",
                TheNVarChar = "Андрей Коваленко",
                TheNVarCharMax = "Роки Марчиано",

                TheBinary = new byte[] { 0x10, 0x20, 0x33 },
                TheVarBinary = new byte[] { 0xff, 0xee, 0xbb },
                TheVarBinaryMax = new byte[] { 0x80, 0x90, 0xa0 },
            });

            // Act
            cruder.InsertRow("SuperTable", row, (Func<string, bool>)(x => true));

            // Assert
            var insertedRow = TestHelper.LoadRow(this.Connection, "zeta", "SuperTable", 1);

            Assert.That(insertedRow["TheGuid"], Is.EqualTo(new Guid("8e816a5f-b97c-43df-95e9-4fbfe7172dd0")));

            Assert.That(insertedRow["TheBit"], Is.EqualTo(true));

            Assert.That(insertedRow["TheTinyInt"], Is.EqualTo((byte)17));
            Assert.That(insertedRow["TheSmallInt"], Is.EqualTo((short)11));
            Assert.That(insertedRow["TheInt"], Is.EqualTo(44));
            Assert.That(insertedRow["TheBigInt"], Is.EqualTo(777L));

            Assert.That(insertedRow["TheDecimal"], Is.EqualTo(11.2m));
            Assert.That(insertedRow["TheNumeric"], Is.EqualTo(22.3m));

            Assert.That(insertedRow["TheSmallMoney"], Is.EqualTo(123.06m));
            Assert.That(insertedRow["TheMoney"], Is.EqualTo(60.77m));

            Assert.That(insertedRow["TheReal"], Is.EqualTo((float)15.99));
            Assert.That(insertedRow["TheFloat"], Is.EqualTo(7001.555));

            Assert.That(insertedRow["TheDate"], Is.EqualTo(DateTime.Parse("2010-01-02")));
            Assert.That(insertedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2011-11-12T10:10:10")));
            Assert.That(insertedRow["TheDateTime2"], Is.EqualTo(DateTime.Parse("2015-03-07T05:06:33.777")));
            Assert.That(insertedRow["TheDateTimeOffset"], Is.EqualTo(DateTimeOffset.Parse("2011-11-12T10:10:10+03:00")));
            Assert.That(insertedRow["TheSmallDateTime"], Is.EqualTo(DateTime.Parse("1970-04-08T11:11")));
            Assert.That(insertedRow["TheTime"], Is.EqualTo(TimeSpan.Parse("03:03:03")));

            Assert.That(insertedRow["TheChar"], Does.StartWith("abc"));
            Assert.That(insertedRow["TheVarChar"], Is.EqualTo("Andrey Kovalenko"));
            Assert.That(insertedRow["TheVarCharMax"], Is.EqualTo("Rocky Marciano"));

            Assert.That(insertedRow["TheNChar"], Does.StartWith("АБВ"));
            Assert.That(insertedRow["TheNVarChar"], Is.EqualTo("Андрей Коваленко"));
            Assert.That(insertedRow["TheNVarCharMax"], Is.EqualTo("Роки Марчиано"));

            CollectionAssert.AreEqual(new byte[] { 0x10, 0x20, 0x33 }, ((byte[])insertedRow["TheBinary"]).Take(3));
            CollectionAssert.AreEqual(new byte[] { 0xff, 0xee, 0xbb }, (byte[])insertedRow["TheVarBinary"]);
            CollectionAssert.AreEqual(new byte[] { 0x80, 0x90, 0xa0 }, (byte[])insertedRow["TheVarBinaryMax"]);
        }

        [Test]
        public void InsertRow_RowIsEmptyAndSelectorIsFalser_InsertsDefaultValues()
        {
            // Arrange
            var row1 = new Dictionary<string, object>();
            var row2 = new DynamicRow();
            var row3 = new { };

            object[] rows =
            {
                row1,
                row2,
                row3,
            };

            var insertedRows = new IReadOnlyDictionary<string, object>[rows.Length];

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            using var command = this.Connection.CreateCommand();

            for (var i = 0; i < rows.Length; i++)
            {
                var row = rows[i];

                var createTableSql = @"
CREATE TABLE [zeta].[MyTab](
    [Id] int NOT NULL PRIMARY KEY IDENTITY(1, 1),
    [Length] int NULL DEFAULT NULL,
    [Name] nvarchar(100) DEFAULT 'Polly')
";
                command.CommandText = createTableSql;
                command.ExecuteNonQuery();

                cruder.InsertRow("MyTab", row, x => false);
                var insertedRow = TestHelper.LoadRow(this.Connection, "zeta", "MyTab", 1);
                insertedRows[i] = insertedRow;

                this.Connection.ExecuteSingleSql("DROP TABLE [zeta].[MyTab]");
            }

            // Assert
            var json = JsonConvert.SerializeObject(
                new
                {
                    Id = 1,
                    Length = (int?)null,
                    Name = "Polly",
                },
                Formatting.Indented);

            foreach (var insertedRow in insertedRows)
            {
                var insertedJson = JsonConvert.SerializeObject(insertedRow, Formatting.Indented);
                Assert.That(insertedJson, Is.EqualTo(json));
            }
        }

        [Test]
        public void InsertRow_RowHasUnknownPropertiesAndSelectorIsFalser_InsertsDefaultValues()
        {
            // Arrange
            this.CreateSmallTable();

            var row1 = new Dictionary<string, object>
            {
                {"NonExisting", 777},
            };

            var row2 = new DynamicRow();
            row2.SetValue("NonExisting", 777);

            var row3 = new
            {
                NonExisting = 777,
            };

            var row4 = new DummyDto
            {
                NonExisting = 777,
            };

            object[] rows =
            {
                row1,
                row2,
                row3,
                row4,
            };

            IReadOnlyDictionary<string, object>[] insertedRows = new IReadOnlyDictionary<string, object>[rows.Length];
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            for (var i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                cruder.InsertRow("SmallTable", row, x => false);

                var lastIdentity = (int)this.Connection.GetLastIdentity();

                var insertedRow = TestHelper.LoadRow(
                    this.Connection,
                    "zeta",
                    "SmallTable",
                    lastIdentity);

                insertedRows[i] = insertedRow;

                this.Connection.ExecuteSingleSql("DELETE FROM [zeta].[SmallTable]");
            }

            // Assert
            foreach (var insertedRow in insertedRows)
            {
                Assert.That(insertedRow["TheInt"], Is.EqualTo(1599));
                Assert.That(insertedRow["TheNVarChar"], Is.EqualTo("Semmi"));
            }
        }

        [Test]
        public void InsertRow_NoColumnForSelectedProperty_ThrowsTauDbException()
        {
            // Arrange
            this.CreateSmallTable();

            var row = new
            {
                TheInt = 1,
                TheNVarChar = "Polina",
                NotExisting = 100,
            };

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRow("SmallTable", row));

            // Assert
            Assert.That(ex, Has.Message.EqualTo($"Column 'NotExisting' does not exist."));
        }

        [Test]
        public void InsertRow_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRow("some_table", new object()));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void InsertRow_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRow("bad_table", new object()));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void InsertRow_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.InsertRow(null, new object(), x => true));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void InsertRow_RowIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.InsertRow("HealthInfo", null, x => true));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("row"));
        }

        [Test]
        public void InsertRow_RowContainsDBNullValue_ThrowsTauDbException()
        {
            // Arrange
            this.CreateSuperTable();
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");
            var row = new
            {
                TheGuid = DBNull.Value,
            };

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRow("SuperTable", row, x => x == "TheGuid"));

            // Assert
            Assert.That(ex, Has.Message.EqualTo("Could not transform value '' of type 'System.DBNull'. Table name is 'SuperTable'. Column name is 'TheGuid'."));
        }

        #endregion

        #region InsertRows

        [Test]
        public void InsertRows_ValidArguments_InsertsRows()
        {
            // Arrange
            var row1 = new Dictionary<string, object>
            {
                {"Id", new Guid("11111111-1111-1111-1111-111111111111")},
                {"PersonId", 101},
                {"Weight", 69.20m},
                {"PersonMetaKey", (short) 12},
                {"IQ", 101.60m},
                {"Temper", (short) 4},
                {"PersonOrdNumber", (byte) 3},
                {"MetricB", -3},
                {"MetricA", 177},
                {"NotExisting", 7},
            };

            var row2 = new DynamicRow();
            row2.SetValue("Id", new Guid("22222222-2222-2222-2222-222222222222"));
            row2.SetValue("PersonId", 101);
            row2.SetValue("Weight", 69.20m);
            row2.SetValue("PersonMetaKey", (short)12);
            row2.SetValue("IQ", 101.60m);
            row2.SetValue("Temper", (short)4);
            row2.SetValue("PersonOrdNumber", (byte)3);
            row2.SetValue("MetricB", -3);
            row2.SetValue("MetricA", 177);
            row2.SetValue("NotExisting", 7);

            var row3 = new
            {
                Id = new Guid("33333333-3333-3333-3333-333333333333"),
                PersonId = 101,
                Weight = 69.20m,
                PersonMetaKey = (short)12,
                IQ = 101.60m,
                Temper = (short)4,
                PersonOrdNumber = (byte)3,
                MetricB = -3,
                MetricA = 177,
                NotExisting = 7,
            };

            var row4 = new HealthInfoDto
            {
                Id = new Guid("44444444-4444-4444-4444-444444444444"),
                PersonId = 101,
                Weight = 69.20m,
                PersonMetaKey = 12,
                IQ = 101.60m,
                Temper = 4,
                PersonOrdNumber = 3,
                MetricB = -3,
                MetricA = 177,
                NotExisting = 7,
            };

            object[] rows =
            {
                row1,
                row2,
                row3,
                row4,
            };

            this.Connection.ExecuteSingleSql("ALTER TABLE [zeta].[HealthInfo] DROP CONSTRAINT [FK_healthInfo_Person]");

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            cruder.InsertRows("HealthInfo", rows, x => x != "NotExisting");

            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    *
FROM
    [zeta].[HealthInfo]
ORDER BY
    [Id]
";
            var loadedRows = DbTools.GetCommandRows(command);
            Assert.That(loadedRows, Has.Count.EqualTo(4));

            for (var i = 0; i < loadedRows.Count; i++)
            {
                var cleanOriginalRow = new DynamicRow(rows[i]);
                cleanOriginalRow.DeleteValue("NotExisting");

                var json = JsonConvert.SerializeObject(cleanOriginalRow, Formatting.Indented);
                var loadedJson = JsonConvert.SerializeObject(loadedRows[i], Formatting.Indented);

                Assert.That(json, Is.EqualTo(loadedJson));
            }
        }

        [Test]
        public void InsertRows_RowsAreEmptyAndSelectorIsFalser_InsertsDefaultValues()
        {
            // Arrange
            this.CreateSmallTable();

            var row1 = new Dictionary<string, object>();
            var row2 = new DynamicRow();
            var row3 = new object();

            var rows = new[]
            {
                row1,
                row2,
                row3,
            };

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            cruder.InsertRows("SmallTable", rows, x => false);

            // Assert
            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    *
FROM
    [zeta].[SmallTable]
ORDER BY
    [Id]
";
            var loadedRows = DbTools.GetCommandRows(command);
            Assert.That(loadedRows, Has.Count.EqualTo(3));

            foreach (var loadedRow in loadedRows)
            {
                Assert.That(loadedRow.TheInt, Is.EqualTo(1599));
                Assert.That(loadedRow.TheNVarChar, Is.EqualTo("Semmi"));
            }
        }

        [Test]
        public void InsertRows_PropertySelectorProducesNoProperties_InsertsDefaultValues()
        {
            // Arrange
            this.CreateSmallTable();

            var row1 = new
            {
                TheInt = 77,
                TheNVarChar = "abc",
            };

            var row2 = new
            {
                TheInt = 88,
                TheNVarChar = "def",
            };

            var rows = new[] { row1, row2 };

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            cruder.InsertRows("SmallTable", rows, x => false);

            // Assert
            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    *
FROM
    [zeta].[SmallTable]
ORDER BY
    [Id]
";

            var loadedRows = DbTools.GetCommandRows(command);
            Assert.That(loadedRows, Has.Count.EqualTo(2));

            foreach (var loadedRow in loadedRows)
            {
                Assert.That(loadedRow.TheInt, Is.EqualTo(1599));
                Assert.That(loadedRow.TheNVarChar, Is.EqualTo("Semmi"));
            }
        }

        [Test]
        public void InsertRows_PropertySelectorIsNull_UsesAllColumns()
        {
            // Arrange
            this.CreateSmallTable();

            var row1 = new
            {
                TheInt = 77,
                TheNVarChar = "abc",
            };

            var row2 = new
            {
                TheInt = 88,
                TheNVarChar = "def",
            };

            var rows = new[] { row1, row2 };

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            cruder.InsertRows("SmallTable", rows);

            // Assert
            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    *
FROM
    [zeta].[SmallTable]
ORDER BY
    [Id]
";

            var loadedRows = DbTools.GetCommandRows(command);
            Assert.That(loadedRows, Has.Count.EqualTo(2));

            var loadedRow = loadedRows[0];
            Assert.That(loadedRow.TheInt, Is.EqualTo(77));
            Assert.That(loadedRow.TheNVarChar, Is.EqualTo("abc"));

            loadedRow = loadedRows[1];
            Assert.That(loadedRow.TheInt, Is.EqualTo(88));
            Assert.That(loadedRow.TheNVarChar, Is.EqualTo("def"));
        }

        [Test]
        public void InsertRows_NoColumnForSelectedProperty_ThrowsTauDbException()
        {
            // Arrange
            this.CreateSmallTable();

            var row1 = new
            {
                TheInt = 77,
                TheNVarChar = "abc",
                NotExisting = 2,
            };

            var row2 = new
            {
                TheInt = 88,
                TheNVarChar = "def",
                NotExisting = 1,
            };

            var rows = new[] { row1, row2 };

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRows("SmallTable", rows, x => true));

            // Assert
            Assert.That(ex, Has.Message.EqualTo("Column 'NotExisting' does not exist."));
        }

        [Test]
        public void InsertRows_NextRowSignatureDiffersFromPrevious_ThrowsArgumentException()
        {
            // Arrange
            this.CreateSmallTable();

            var row1 = new
            {
                TheInt = 77,
            };

            var row2 = new
            {
                TheNVarChar = "def",
            };

            var rows = new object[] { row1, row2 };

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentException>(() => cruder.InsertRows("SmallTable", rows, x => true));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("'values' does not contain property representing column 'TheInt'."));
        }

        [Test]
        public void InsertRows_SchemaDoesNotExist_TauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRows("some_table", new object[] { }));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void InsertRows_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRows("bad_table", new object[] { }));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void InsertRows_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.InsertRows(null, new object[] { }));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void InsertRows_RowsIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.InsertRows("HealthInfo", null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("rows"));
        }

        [Test]
        public void InsertRows_RowsContainNull_ThrowsArgumentException()
        {
            // Arrange
            this.CreateSmallTable();

            var rows = new[]
            {
                new object(),
                null,
            };

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentException>(() => cruder.InsertRows("SmallTable", rows));

            // Assert
            Assert.That(ex, Has.Message.StartWith("'rows' must not contain nulls."));
            Assert.That(ex.ParamName, Is.EqualTo("rows"));
        }

        [Test]
        public void InsertRows_RowContainsDBNullValue_ThrowsTauDbException()
        {
            // Arrange
            this.CreateSmallTable();

            var rows = new object[]
            {
                new
                {
                    TheInt = 10,
                },
                new 
                {
                    TheInt = DBNull.Value,
                },
            };

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRows("SmallTable", rows));

            // Assert
            Assert.That(ex, Has.Message.StartWith("Could not transform value '' of type 'System.DBNull'. Table name is 'SmallTable'. Column name is 'TheInt'."));
        }

        #endregion

        #region RowInsertedCallback

        [Test]
        public void RowInsertedCallback_SetToSomeValue_KeepsThatValue()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void RowInsertedCallback_SetToNonNull_IsCalledWhenInsertRowIsCalled()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void RowInsertedCallback_SetToNonNull_IsCalledWhenInsertRowsIsCalled()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region GetRow

        [Test]
        public void GetRow_ValidArguments_ReturnsRow()
        {
            // Arrange
            this.CreateSuperTable();

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            dynamic row = new DynamicRow(new
            {
                TheGuid = new Guid("8e816a5f-b97c-43df-95e9-4fbfe7172dd0"),

                TheBit = true,

                TheTinyInt = (byte)17,
                TheSmallInt = (short)11,
                TheInt = 44,
                TheBigInt = 777L,

                TheDecimal = 11.2m,
                TheNumeric = 22.3m,

                TheSmallMoney = 123.06m,
                TheMoney = 60.77m,

                TheReal = (float)15.99,
                TheFloat = 7001.555,

                TheDate = DateTime.Parse("2010-01-02"),
                TheDateTime = DateTime.Parse("2011-11-12T10:10:10"),
                TheDateTime2 = DateTime.Parse("2015-03-07T05:06:33.777"),
                TheDateTimeOffset = DateTimeOffset.Parse("2011-11-12T10:10:10+03:00"),
                TheSmallDateTime = DateTime.Parse("1970-04-08T11:11:11"),
                TheTime = TimeSpan.Parse("03:03:03"),

                TheChar = "abc",
                TheVarChar = "Andrey Kovalenko",
                TheVarCharMax = "Rocky Marciano",

                TheNChar = "АБВ",
                TheNVarChar = "Андрей Коваленко",
                TheNVarCharMax = "Роки Марчиано",

                TheBinary = new byte[] { 0x10, 0x20, 0x33 },
                TheVarBinary = new byte[] { 0xff, 0xee, 0xbb },
                TheVarBinaryMax = new byte[] { 0x80, 0x90, 0xa0 },
            });

            cruder.InsertRow("SuperTable", row, (Func<string, bool>)(x => true)); // InsertRow is ut'ed already :)

            // Act
            var insertedRow = ((DynamicRow)cruder.GetRow("SuperTable", 1, x => x.Contains("DateTime"))).ToDictionary();

            // Assert
            Assert.That(insertedRow, Has.Count.EqualTo(4));

            Assert.That(insertedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2011-11-12T10:10:10")));
            Assert.That(insertedRow["TheDateTime2"], Is.EqualTo(DateTime.Parse("2015-03-07T05:06:33.777")));
            Assert.That(insertedRow["TheDateTimeOffset"], Is.EqualTo(DateTimeOffset.Parse("2011-11-12T10:10:10+03:00")));
            Assert.That(insertedRow["TheSmallDateTime"], Is.EqualTo(DateTime.Parse("1970-04-08T11:11")));
        }

        [Test]
        public void GetRow_AllDataTypes_RunsOk()
        {
            // Arrange
            this.CreateSuperTable();

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            dynamic row = new DynamicRow(new
            {
                TheGuid = new Guid("8e816a5f-b97c-43df-95e9-4fbfe7172dd0"),

                TheBit = true,

                TheTinyInt = (byte)17,
                TheSmallInt = (short)11,
                TheInt = 44,
                TheBigInt = 777L,

                TheDecimal = 11.2m,
                TheNumeric = 22.3m,

                TheSmallMoney = 123.06m,
                TheMoney = 60.77m,

                TheReal = (float)15.99,
                TheFloat = 7001.555,

                TheDate = DateTime.Parse("2010-01-02"),
                TheDateTime = DateTime.Parse("2011-11-12T10:10:10"),
                TheDateTime2 = DateTime.Parse("2015-03-07T05:06:33.777"),
                TheDateTimeOffset = DateTimeOffset.Parse("2011-11-12T10:10:10+03:00"),
                TheSmallDateTime = DateTime.Parse("1970-04-08T11:11:11"),
                TheTime = TimeSpan.Parse("03:03:03"),

                TheChar = "abc",
                TheVarChar = "Andrey Kovalenko",
                TheVarCharMax = "Rocky Marciano",

                TheNChar = "АБВ",
                TheNVarChar = "Андрей Коваленко",
                TheNVarCharMax = "Роки Марчиано",

                TheBinary = new byte[] { 0x10, 0x20, 0x33 },
                TheVarBinary = new byte[] { 0xff, 0xee, 0xbb },
                TheVarBinaryMax = new byte[] { 0x80, 0x90, 0xa0 },
            });

            cruder.InsertRow("SuperTable", row, (Func<string, bool>)(x => true)); // InsertRow is ut'ed already :)

            // Act
            var insertedRow = ((DynamicRow)cruder.GetRow("SuperTable", 1)).ToDictionary();

            // Assert
            Assert.That(insertedRow["TheGuid"], Is.EqualTo(new Guid("8e816a5f-b97c-43df-95e9-4fbfe7172dd0")));

            Assert.That(insertedRow["TheBit"], Is.EqualTo(true));

            Assert.That(insertedRow["TheTinyInt"], Is.EqualTo((byte)17));
            Assert.That(insertedRow["TheSmallInt"], Is.EqualTo((short)11));
            Assert.That(insertedRow["TheInt"], Is.EqualTo(44));
            Assert.That(insertedRow["TheBigInt"], Is.EqualTo(777L));

            Assert.That(insertedRow["TheDecimal"], Is.EqualTo(11.2m));
            Assert.That(insertedRow["TheNumeric"], Is.EqualTo(22.3m));

            Assert.That(insertedRow["TheSmallMoney"], Is.EqualTo(123.06m));
            Assert.That(insertedRow["TheMoney"], Is.EqualTo(60.77m));

            Assert.That(insertedRow["TheReal"], Is.EqualTo((float)15.99));
            Assert.That(insertedRow["TheFloat"], Is.EqualTo(7001.555));

            Assert.That(insertedRow["TheDate"], Is.EqualTo(DateTime.Parse("2010-01-02")));
            Assert.That(insertedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2011-11-12T10:10:10")));
            Assert.That(insertedRow["TheDateTime2"], Is.EqualTo(DateTime.Parse("2015-03-07T05:06:33.777")));
            Assert.That(insertedRow["TheDateTimeOffset"], Is.EqualTo(DateTimeOffset.Parse("2011-11-12T10:10:10+03:00")));
            Assert.That(insertedRow["TheSmallDateTime"], Is.EqualTo(DateTime.Parse("1970-04-08T11:11")));
            Assert.That(insertedRow["TheTime"], Is.EqualTo(TimeSpan.Parse("03:03:03")));

            Assert.That(insertedRow["TheChar"], Does.StartWith("abc"));
            Assert.That(insertedRow["TheVarChar"], Is.EqualTo("Andrey Kovalenko"));
            Assert.That(insertedRow["TheVarCharMax"], Is.EqualTo("Rocky Marciano"));

            Assert.That(insertedRow["TheNChar"], Does.StartWith("АБВ"));
            Assert.That(insertedRow["TheNVarChar"], Is.EqualTo("Андрей Коваленко"));
            Assert.That(insertedRow["TheNVarCharMax"], Is.EqualTo("Роки Марчиано"));

            CollectionAssert.AreEqual(new byte[] { 0x10, 0x20, 0x33 }, ((byte[])insertedRow["TheBinary"]).Take(3));
            CollectionAssert.AreEqual(new byte[] { 0xff, 0xee, 0xbb }, (byte[])insertedRow["TheVarBinary"]);
            CollectionAssert.AreEqual(new byte[] { 0x80, 0x90, 0xa0 }, (byte[])insertedRow["TheVarBinaryMax"]);
        }

        [Test]
        public void GetRow_SelectorIsTruer_DeliversAllColumns()
        {
            // Arrange
            this.CreateSuperTable();

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            dynamic row = new DynamicRow(new
            {
                TheGuid = new Guid("8e816a5f-b97c-43df-95e9-4fbfe7172dd0"),

                TheBit = true,

                TheTinyInt = (byte)17,
                TheSmallInt = (short)11,
                TheInt = 44,
                TheBigInt = 777L,

                TheDecimal = 11.2m,
                TheNumeric = 22.3m,

                TheSmallMoney = 123.06m,
                TheMoney = 60.77m,

                TheReal = (float)15.99,
                TheFloat = 7001.555,

                TheDate = DateTime.Parse("2010-01-02"),
                TheDateTime = DateTime.Parse("2011-11-12T10:10:10"),
                TheDateTime2 = DateTime.Parse("2015-03-07T05:06:33.777"),
                TheDateTimeOffset = DateTimeOffset.Parse("2011-11-12T10:10:10+03:00"),
                TheSmallDateTime = DateTime.Parse("1970-04-08T11:11:11"),
                TheTime = TimeSpan.Parse("03:03:03"),

                TheChar = "abc",
                TheVarChar = "Andrey Kovalenko",
                TheVarCharMax = "Rocky Marciano",

                TheNChar = "АБВ",
                TheNVarChar = "Андрей Коваленко",
                TheNVarCharMax = "Роки Марчиано",

                TheBinary = new byte[] { 0x10, 0x20, 0x33 },
                TheVarBinary = new byte[] { 0xff, 0xee, 0xbb },
                TheVarBinaryMax = new byte[] { 0x80, 0x90, 0xa0 },
            });

            cruder.InsertRow("SuperTable", row, (Func<string, bool>)(x => true)); // InsertRow is ut'ed already :)

            // Act
            var insertedRow = ((DynamicRow)cruder.GetRow("SuperTable", 1, x => true)).ToDictionary();

            // Assert
            Assert.That(insertedRow["TheGuid"], Is.EqualTo(new Guid("8e816a5f-b97c-43df-95e9-4fbfe7172dd0")));

            Assert.That(insertedRow["TheBit"], Is.EqualTo(true));

            Assert.That(insertedRow["TheTinyInt"], Is.EqualTo((byte)17));
            Assert.That(insertedRow["TheSmallInt"], Is.EqualTo((short)11));
            Assert.That(insertedRow["TheInt"], Is.EqualTo(44));
            Assert.That(insertedRow["TheBigInt"], Is.EqualTo(777L));

            Assert.That(insertedRow["TheDecimal"], Is.EqualTo(11.2m));
            Assert.That(insertedRow["TheNumeric"], Is.EqualTo(22.3m));

            Assert.That(insertedRow["TheSmallMoney"], Is.EqualTo(123.06m));
            Assert.That(insertedRow["TheMoney"], Is.EqualTo(60.77m));

            Assert.That(insertedRow["TheReal"], Is.EqualTo((float)15.99));
            Assert.That(insertedRow["TheFloat"], Is.EqualTo(7001.555));

            Assert.That(insertedRow["TheDate"], Is.EqualTo(DateTime.Parse("2010-01-02")));
            Assert.That(insertedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2011-11-12T10:10:10")));
            Assert.That(insertedRow["TheDateTime2"], Is.EqualTo(DateTime.Parse("2015-03-07T05:06:33.777")));
            Assert.That(insertedRow["TheDateTimeOffset"], Is.EqualTo(DateTimeOffset.Parse("2011-11-12T10:10:10+03:00")));
            Assert.That(insertedRow["TheSmallDateTime"], Is.EqualTo(DateTime.Parse("1970-04-08T11:11")));
            Assert.That(insertedRow["TheTime"], Is.EqualTo(TimeSpan.Parse("03:03:03")));

            Assert.That(insertedRow["TheChar"], Does.StartWith("abc"));
            Assert.That(insertedRow["TheVarChar"], Is.EqualTo("Andrey Kovalenko"));
            Assert.That(insertedRow["TheVarCharMax"], Is.EqualTo("Rocky Marciano"));

            Assert.That(insertedRow["TheNChar"], Does.StartWith("АБВ"));
            Assert.That(insertedRow["TheNVarChar"], Is.EqualTo("Андрей Коваленко"));
            Assert.That(insertedRow["TheNVarCharMax"], Is.EqualTo("Роки Марчиано"));

            CollectionAssert.AreEqual(new byte[] { 0x10, 0x20, 0x33 }, ((byte[])insertedRow["TheBinary"]).Take(3));
            CollectionAssert.AreEqual(new byte[] { 0xff, 0xee, 0xbb }, (byte[])insertedRow["TheVarBinary"]);
            CollectionAssert.AreEqual(new byte[] { 0x80, 0x90, 0xa0 }, (byte[])insertedRow["TheVarBinaryMax"]);
        }

        [Test]
        public void GetRow_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetRow("some_table", 1));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetRow_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetRow("bad_table", 1));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void GetRow_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.GetRow(null, 1));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void GetRow_IdIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.GetRow("some_table", null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("id"));
        }

        [Test]
        public void GetRow_TableHasNoPrimaryKey_ArgumentException()

        {
            // Arrange
            this.Connection.ExecuteSingleSql("CREATE TABLE [zeta].[dummy](Foo int)"); // no PK
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentException>((() => cruder.GetRow("dummy", 1)));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("Table 'dummy' does not have a primary key."));
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }
        [Test]
        public void GetRow_TablePrimaryKeyIsMultiColumn_ThrowsArgumentException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentException>((() => cruder.GetRow("Person", "the_id")));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("Failed to retrieve single primary key column name for table 'Person'."));
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void GetRow_IdNotFound_ReturnsNull()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");
            const int nonExistingId = 133;

            // Act
            var row = cruder.GetRow("NumericData", nonExistingId);

            // Assert
            Assert.That(row, Is.Null);
        }

        [Test]
        public void GetRow_SelectorIsFalser_ArgumentException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentException>(() => cruder.GetRow("NumericData", 111, x => false));

            // Assert
            Assert.That(ex, Has.Message.StartWith("No columns were selected."));
            Assert.That(ex.ParamName, Is.EqualTo("columnSelector"));
        }

        #endregion

        #region GetAllRows

        [Test]
        public void GetAllRows_ValidArguments_ReturnsRows()
        {
            // Arrange
            var insertSql = this.GetType().Assembly.GetResourceText("InsertRows.sql", true);
            this.Connection.ExecuteCommentedScript(insertSql);

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var rows = cruder.GetAllRows("DateData", x => x == "Moment");

            // Assert
            var row = (DynamicRow)rows[0];
            Assert.That(row.GetDynamicMemberNames().Count(), Is.EqualTo(1));
            Assert.That(row.GetValue("Moment"), Is.EqualTo(DateTimeOffset.Parse("2020-01-01T05:05:05+00:00")));

            row = rows[1];
            Assert.That(row.GetDynamicMemberNames().Count(), Is.EqualTo(1));
            Assert.That(row.GetValue("Moment"), Is.EqualTo(DateTimeOffset.Parse("2020-02-02T06:06:06+00:00")));
        }
        [Test]
        public void GetAllRows_SelectorIsTruer_DeliversAllColumns()
        {
            // Arrange
            var insertSql = this.GetType().Assembly.GetResourceText("InsertRows.sql", true);
            this.Connection.ExecuteCommentedScript(insertSql);

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var rows = cruder.GetAllRows("DateData", x => true);

            // Assert
            var row = rows[0];
            Assert.That(row.Id, Is.EqualTo(new Guid("11111111-1111-1111-1111-111111111111")));
            Assert.That(row.Moment, Is.EqualTo(DateTimeOffset.Parse("2020-01-01T05:05:05+00:00")));

            row = rows[1];
            Assert.That(row.Id, Is.EqualTo(new Guid("22222222-2222-2222-2222-222222222222")));
            Assert.That(row.Moment, Is.EqualTo(DateTimeOffset.Parse("2020-02-02T06:06:06+00:00")));
        }

        [Test]
        public void GetAllRows_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetAllRows("some_table"));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetAllRows_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetAllRows("bad_table"));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void GetAllRows_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.GetAllRows(null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void GetAllRows_SelectorIsFalser_ThrowsArgumentException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentException>(() => cruder.GetAllRows("HealthInfo", x => false));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("No columns were selected."));
            Assert.That(ex.ParamName, Is.EqualTo("columnSelector"));
        }

        #endregion

        #region UpdateRow

        [Test]
        public void UpdateRow_ValidArguments_UpdatesRow()
        {
            // Arrange

            // Act
            // todo: row is a dictionary, row is a DynamicRow, row is an anon type, row is a strongly-typed dto

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void UpdateRow_AllDataTypes_RunsOk()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }


        [Test]
        public void UpdateRow_SchemaDoesNotExist_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void UpdateRow_TableDoesNotExist_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void UpdateRow_TableNameIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void UpdateRow_RowUpdateIsNull_UpdatesRow()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void UpdateRow_PropertySelectorIsNull_UsesAllProperties()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void UpdateRow_RowUpdateContainsPropertiesOnWhichSelectorReturnsFalse_RunsOk()
        {
            // Arrange

            // todo: row is a dictionary, row is a DynamicRow, row is an anon type, row is a strongly-typed dto

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void UpdateRow_PropertySelectorDoesNotContainPkColumn_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void UpdateRow_PropertySelectorContainsOnlyPkColumn_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void UpdateRow_IdIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void UpdateRow_NoColumnForSelectedProperty_ThrowsTodo()
        {
            // Arrange

            // todo: row is a dictionary, row is a DynamicRow, row is an anon type, row is a strongly-typed dto

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void UpdateRow_TableHasNoPrimaryKey_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void UpdateRow_TablePrimaryKeyIsMultiColumn_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region DeleteRow

        [Test]
        public void DeleteRow_ValidArguments_DeletesRowAndReturnsTrue()
        {
            // Arrange
            this.CreateMediumTable();
            const int id = 1;
            this.Connection.ExecuteSingleSql($"INSERT INTO [zeta].[MediumTable]([Id]) VALUES ({id})");

            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var deleted = cruder.DeleteRow("MediumTable", id);

            // Assert
            var deletedRow = TestHelper.LoadRow(this.Connection, "zeta", "MediumTable", id);

            Assert.That(deleted, Is.True);
            Assert.That(deletedRow, Is.Null);
        }

        [Test]
        public void DeleteRow_IdNotFound_ReturnsFalse()
        {
            // Arrange
            this.CreateMediumTable();
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");
            var notExistingId = 11;

            // Act
            var deleted = cruder.DeleteRow("MediumTable", notExistingId);

            // Assert
            Assert.That(deleted, Is.False);
        }

        [Test]
        public void DeleteRow_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.DeleteRow("some_table", 17));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void DeleteRow_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.DeleteRow("bad_table", 17));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void DeleteRow_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.DeleteRow(null, 11));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void DeleteRow_IdIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.DeleteRow("MediumTable", null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("id"));
        }

        [Test]
        public void DeleteRow_TableHasNoPrimaryKey_ThrowsArgumentException()
        {
            // Arrange
            this.Connection.ExecuteSingleSql("CREATE TABLE [zeta].[dummy](Foo int)"); // no PK
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act
            var ex = Assert.Throws<ArgumentException>((() => cruder.DeleteRow("dummy", 1)));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("Table 'dummy' does not have a primary key."));
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void DeleteRow_PrimaryKeyIsMultiColumn_ThrowsArgumentException()
        {
            // Arrange
            IDbCruder cruder = new SqlCruderLab(this.Connection, "zeta");

            // Act

            var ex = Assert.Throws<ArgumentException>((() => cruder.DeleteRow("Person", "the_id")));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("Failed to retrieve single primary key column name for table 'Person'."));
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        #endregion
    }
}
