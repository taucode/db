using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TauCode.Db.Data;
using TauCode.Db.DbValueConverters;
using TauCode.Db.Exceptions;
using TauCode.Db.MySql.DbValueConverters;
using TauCode.Extensions;

namespace TauCode.Db.MySql.Tests.DbCruder
{
    [TestFixture]
    public class MySqlCruderTests : TestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Connection.CreateSchema("zeta");

            var sql = this.GetType().Assembly.GetResourceText("crebase.sql", true);
            this.Connection.ExecuteCommentedScript(sql);
        }

        private void CreateSuperTable()
        {
            var sql = this.GetType().Assembly.GetResourceText("SuperTable.sql", true);
            this.Connection.ExecuteSingleSql(sql);
        }

        private SuperTableRowDto CreateSuperTableRowDto()
        {
            return new SuperTableRowDto
            {
                TheInt = -13,
                TheIntUnsigned = 13,

                TheBit = 1,
                TheBit9 = 257,
                TheBit17 = 65536,
                TheBit33 = (ulong)int.MaxValue + 10,

                TheTinyInt = (sbyte)-17,
                TheTinyIntUnsigned = (byte)17,

                TheBool = 0,
                TheBoolean = 1,

                TheSmallInt = (short)-777,
                TheSmallIntUnsigned = 777,

                TheMediumInt = -65536,
                TheMediumIntUnsigned = 65536,

                TheBigInt = (long)int.MinValue - 1,
                TheBigIntUnsigned = (ulong)int.MaxValue + 1,

                TheDecimal = 11.2m,
                TheNumeric = 22.3m,

                TheFloat = 1.2f,
                TheDouble = 101.3d,

                TheDate = DateTime.Parse("2010-01-02"),
                TheDateTime = DateTime.Parse("2011-11-12T10:10:10"),
                TheTimeStamp = DateTime.Parse("2015-03-07T05:06:33.777"),
                TheTime = TimeSpan.Parse("03:03:03"),
                TheYear = 1917,

                TheChar = "abc",
                TheVarChar = "Андрей Коваленко",

                TheBinary = new byte[] { 0x10, 0x20, 0x33 },
                TheVarBinary = new byte[] { 0xff, 0xee, 0xbb },

                TheTinyText = "ze tiny text",
                TheText = "Зе текст",
                TheMediumText = "ze medium text",
                TheLongText = "Зе лонг текст",

                TheTinyBlob = new byte[] { 0x10, 0x11, 0x12 },
                TheBlob = new byte[] { 0x20, 0x21, 0x22 },
                TheMediumBlob = new byte[] { 0x30, 0x31, 0x32 },
                TheLongBlob = new byte[] { 0x40, 0x41, 0x42 },
            };
        }

        private void InsertSuperTableRow()
        {
            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
INSERT INTO zeta.SuperTable(
    `TheInt`,
    `TheIntUnsigned`,

    `TheBit`,
    `TheBit9`,
    `TheBit17`,
    `TheBit33`,

    `TheTinyInt`,
    `TheTinyIntUnsigned`,

    `TheBool`,
    `TheBoolean`,

    `TheSmallInt`,
    `TheSmallIntUnsigned`,
    
    `TheMediumInt`,
    `TheMediumIntUnsigned`,

    `TheBigInt`,
    `TheBigIntUnsigned`,

    `TheDecimal`,
    `TheNumeric`,

    `TheFloat`,
    `TheDouble`,

    `TheDate`,
    `TheDateTime`,
    `TheTimeStamp`,
    `TheTime`,
    `TheYear`,

    `TheChar`,
    `TheVarChar`,

    `TheBinary`,
    `TheVarBinary`,

    `TheTinyText`,
    `TheText`,
    `TheMediumText`,
    `TheLongText`,

    `TheTinyBlob`,
    `TheBlob`,
    `TheMediumBlob`,
    `TheLongBlob`)
VALUES(
    @p_theInt,
    @p_theIntUnsigned,

    @p_theBit,
    @p_theBit9,
    @p_theBit17,
    @p_theBit33,

    @p_theTinyInt,
    @p_theTinyIntUnsigned,

    @p_theBool,
    @p_theBoolean,

    @p_theSmallInt,
    @p_theSmallIntUnsigned,
    
    @p_theMediumInt,
    @p_theMediumIntUnsigned,

    @p_theBigInt,
    @p_theBigIntUnsigned,

    @p_theDecimal,
    @p_theNumeric,

    @p_theFloat,
    @p_theDouble,

    @p_theDate,
    @p_theDateTime,
    @p_theTimeStamp,
    @p_theTime,
    @p_theYear,

    @p_theChar,
    @p_theVarChar,

    @p_theBinary,
    @p_theVarBinary,

    @p_theTinyText,
    @p_theText,
    @p_theMediumText,
    @p_theLongText,

    @p_theTinyBlob,
    @p_theBlob,
    @p_theMediumBlob,
    @p_theLongBlob)
";

            var row = this.CreateSuperTableRowDto();

            command.Parameters.AddWithValue("@p_theInt", row.TheInt);
            command.Parameters.AddWithValue("@p_theIntUnsigned", row.TheIntUnsigned);

            command.Parameters.AddWithValue("@p_theBit", row.TheBit);
            command.Parameters.AddWithValue("@p_theBit9", row.TheBit9);
            command.Parameters.AddWithValue("@p_theBit17", row.TheBit17);
            command.Parameters.AddWithValue("@p_theBit33", row.TheBit33);

            command.Parameters.AddWithValue("@p_theTinyInt", row.TheTinyInt);
            command.Parameters.AddWithValue("@p_theTinyIntUnsigned", row.TheTinyIntUnsigned);

            command.Parameters.AddWithValue("@p_theBool", row.TheBool);
            command.Parameters.AddWithValue("@p_theBoolean", row.TheBoolean);

            command.Parameters.AddWithValue("@p_theSmallInt", row.TheSmallInt);
            command.Parameters.AddWithValue("@p_theSmallIntUnsigned", row.TheSmallIntUnsigned);

            command.Parameters.AddWithValue("@p_theMediumInt", row.TheMediumInt);
            command.Parameters.AddWithValue("@p_theMediumIntUnsigned", row.TheMediumIntUnsigned);

            command.Parameters.AddWithValue("@p_theBigInt", row.TheBigInt);
            command.Parameters.AddWithValue("@p_theBigIntUnsigned", row.TheBigIntUnsigned);

            command.Parameters.AddWithValue("@p_theDecimal", row.TheDecimal);
            command.Parameters.AddWithValue("@p_theNumeric", row.TheNumeric);

            command.Parameters.AddWithValue("@p_theFloat", row.TheFloat);
            command.Parameters.AddWithValue("@p_theDouble", row.TheDouble);

            command.Parameters.AddWithValue("@p_theDate", row.TheDate);
            command.Parameters.AddWithValue("@p_theDateTime", row.TheDateTime);
            command.Parameters.AddWithValue("@p_theTimeStamp", row.TheTimeStamp);
            command.Parameters.AddWithValue("@p_theTime", row.TheTime);
            command.Parameters.AddWithValue("@p_theYear", row.TheYear);

            command.Parameters.AddWithValue("@p_theChar", row.TheChar);
            command.Parameters.AddWithValue("@p_theVarChar", row.TheVarChar);

            command.Parameters.AddWithValue("@p_theBinary", row.TheBinary);
            command.Parameters.AddWithValue("@p_theVarBinary", row.TheVarBinary);

            command.Parameters.AddWithValue("@p_theTinyText", row.TheTinyText);
            command.Parameters.AddWithValue("@p_theText", row.TheText);
            command.Parameters.AddWithValue("@p_theMediumText", row.TheMediumText);
            command.Parameters.AddWithValue("@p_theLongText", row.TheLongText);

            command.Parameters.AddWithValue("@p_theTinyBlob", row.TheTinyBlob);
            command.Parameters.AddWithValue("@p_theBlob", row.TheBlob);
            command.Parameters.AddWithValue("@p_theMediumBlob", row.TheMediumBlob);
            command.Parameters.AddWithValue("@p_theLongBlob", row.TheLongBlob);

            command.ExecuteNonQuery();
        }

        private void CreateMediumTable()
        {
            var sql = @"
CREATE TABLE `zeta`.`MediumTable`(
    `Id` int NOT NULL PRIMARY KEY,

    `TheInt` int NULL DEFAULT 1599,
    `TheNVarChar` nvarchar(100) NULL DEFAULT 'Semmi')
";

            this.Connection.ExecuteSingleSql(sql);
        }

        private void CreateSmallTable()
        {
            var sql = @"
CREATE TABLE `zeta`.`SmallTable`(
    `Id` int NOT NULL PRIMARY KEY AUTO_INCREMENT,

    `TheInt` int NULL DEFAULT 1599,
    `TheNVarChar` nvarchar(100) NULL DEFAULT 'Semmi')
";

            this.Connection.ExecuteSingleSql(sql);
        }

        #region Constructor

        [Test]
        [TestCase("foo")]
        [TestCase(null)]
        public void Constructor_ValidArguments_RunsOk(string schemaName)
        {
            // Arrange

            // Act
            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Assert
            Assert.That(cruder.Connection, Is.SameAs(this.Connection));
            Assert.That(cruder.Factory, Is.SameAs(MySqlUtilityFactory.Instance));
            Assert.That(cruder.SchemaName, Is.EqualTo("foo"));
            Assert.That(cruder.ScriptBuilder, Is.TypeOf<MySqlScriptBuilder>());
            Assert.That(cruder.RowInsertedCallback, Is.Null);
        }

        [Test]
        public void Constructor_ConnectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new MySqlCruder(null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
        }

        [Test]
        public void Constructor_ConnectionIsNotOpen_ThrowsArgumentException()
        {
            // Arrange
            using var connection = new MySqlConnection(TestHelper.ConnectionString);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => new MySqlCruder(connection));

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
            this.CreateSuperTable();

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var converter = cruder.GetTableValuesConverter("SuperTable");

            // Assert
            var dbValueConverter = converter.GetColumnConverter("TheBigIntUnsigned");
            Assert.That(dbValueConverter, Is.TypeOf<UInt64ValueConverter>());
        }

        [Test]
        public void GetTableValuesConverter_ArgumentIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.GetTableValuesConverter(null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void GetTableValuesConverter_NotExistingTable_ThrowsTauDbException()
        {
            // Arrange

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetTableValuesConverter("bad_table"));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        #endregion

        #region ResetTables

        [Test]
        public void ResetTables_NoArguments_RunsOk()
        {
            // Arrange
            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);
            cruder.GetTableValuesConverter("PersonData").SetColumnConverter("Id", new UInt64ValueConverter());
            var oldDbValueConverter = cruder.GetTableValuesConverter("PersonData").GetColumnConverter("Id");

            // Act
            cruder.ResetTables();
            var resetDbValueConverter = cruder.GetTableValuesConverter("PersonData").GetColumnConverter("Id");

            // Assert
            Assert.That(oldDbValueConverter, Is.TypeOf<UInt64ValueConverter>());
            Assert.That(resetDbValueConverter, Is.TypeOf<StringValueConverter>());
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

            this.Connection.ExecuteSingleSql("ALTER TABLE `zeta`.`HealthInfo` DROP CONSTRAINT `FK_healthInfo_Person`");

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);
            cruder.GetTableValuesConverter("HealthInfo").SetColumnConverter("Id", new MySqlGuidValueConverter(MySqlGuidBehaviour.Char36));

            // Act
            for (var i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                cruder.InsertRow("HealthInfo", row, x => x != "NotExisting");
                var loadedRow = TestHelper.LoadRow(
                    this.Connection,
                    "HealthInfo",
                    new Guid("a776fd76-f2a8-4e09-9e69-b6d08e96c075"));

                loadedRows[i] = loadedRow;

                this.Connection.ExecuteSingleSql("DELETE FROM `zeta`.`HealthInfo`");
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

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            dynamic row = new DynamicRow(new
            {
                TheInt = -13,
                TheIntUnsigned = 13,

                TheBit = 1,
                TheBit9 = 257,
                TheBit17 = 65536,
                TheBit33 = (ulong)int.MaxValue + 10,

                TheTinyInt = (sbyte)-17,
                TheTinyIntUnsigned = (byte)17,

                TheBool = 0,
                TheBoolean = 1,

                TheSmallInt = (short)-777,
                TheSmallIntUnsigned = 777,

                TheMediumInt = -65536,
                TheMediumIntUnsigned = 65536,

                TheBigInt = (long)int.MinValue - 1,
                TheBigIntUnsigned = (ulong)int.MaxValue + 1,

                TheDecimal = 11.2m,
                TheNumeric = 22.3m,

                TheFloat = 1.2f,
                TheDouble = 101.3d,

                TheDate = DateTime.Parse("2010-01-02"),
                TheDateTime = DateTime.Parse("2011-11-12T10:10:10"),
                TheTimeStamp = DateTime.Parse("2015-03-07T05:06:33.777"),
                TheTime = TimeSpan.Parse("03:03:03"),
                TheYear = 1917,

                TheChar = "abc",
                TheVarChar = "Андрей Коваленко",

                TheBinary = new byte[] { 0x10, 0x20, 0x33 },
                TheVarBinary = new byte[] { 0xff, 0xee, 0xbb },

                TheTinyText = "ze tiny text",
                TheText = "Зе текст",
                TheMediumText = "ze medium text",
                TheLongText = "Зе лонг текст",

                TheTinyBlob = new byte[] { 0x10, 0x11, 0x12 },
                TheBlob = new byte[] { 0x20, 0x21, 0x22 },
                TheMediumBlob = new byte[] { 0x30, 0x31, 0x32 },
                TheLongBlob = new byte[] { 0x40, 0x41, 0x42 },
            });

            // Act
            cruder.InsertRow("SuperTable", row, (Func<string, bool>)(x => true));

            // Assert
            var loadedRow = TestHelper.LoadRow(this.Connection, "SuperTable", -13);

            Assert.That(loadedRow["TheInt"], Is.EqualTo(-13));
            Assert.That(loadedRow["TheInt"], Is.TypeOf<int>());

            Assert.That(loadedRow["TheIntUnsigned"], Is.EqualTo(13));
            Assert.That(loadedRow["TheIntUnsigned"], Is.TypeOf<uint>());

            Assert.That(loadedRow["TheBit"], Is.EqualTo(1));
            Assert.That(loadedRow["TheBit"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheBit9"], Is.EqualTo(257));
            Assert.That(loadedRow["TheBit9"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheBit17"], Is.EqualTo(65536));
            Assert.That(loadedRow["TheBit17"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheBit33"], Is.EqualTo((ulong)int.MaxValue + 10));
            Assert.That(loadedRow["TheBit33"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheTinyInt"], Is.EqualTo(-17));
            Assert.That(loadedRow["TheTinyInt"], Is.TypeOf<sbyte>());

            Assert.That(loadedRow["TheTinyIntUnsigned"], Is.EqualTo(17));
            Assert.That(loadedRow["TheTinyIntUnsigned"], Is.TypeOf<byte>());

            Assert.That(loadedRow["TheBool"], Is.EqualTo(false));
            Assert.That(loadedRow["TheBool"], Is.TypeOf<bool>());

            Assert.That(loadedRow["TheBoolean"], Is.EqualTo(true));
            Assert.That(loadedRow["TheBoolean"], Is.TypeOf<bool>());

            Assert.That(loadedRow["TheSmallInt"], Is.EqualTo(-777));
            Assert.That(loadedRow["TheSmallInt"], Is.TypeOf<short>());

            Assert.That(loadedRow["TheSmallIntUnsigned"], Is.EqualTo(777));
            Assert.That(loadedRow["TheSmallIntUnsigned"], Is.TypeOf<ushort>());

            Assert.That(loadedRow["TheMediumInt"], Is.EqualTo(-65536));
            Assert.That(loadedRow["TheMediumInt"], Is.TypeOf<int>());

            Assert.That(loadedRow["TheMediumIntUnsigned"], Is.EqualTo(65536));
            Assert.That(loadedRow["TheMediumIntUnsigned"], Is.TypeOf<uint>());

            Assert.That(loadedRow["TheBigInt"], Is.EqualTo((long)int.MinValue - 1));
            Assert.That(loadedRow["TheBigInt"], Is.TypeOf<long>());

            Assert.That(loadedRow["TheBigIntUnsigned"], Is.EqualTo((ulong)int.MaxValue + 1));
            Assert.That(loadedRow["TheBigIntUnsigned"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheDecimal"], Is.EqualTo(11.2m));
            Assert.That(loadedRow["TheDecimal"], Is.TypeOf<decimal>());

            Assert.That(loadedRow["TheNumeric"], Is.EqualTo(22.3m));
            Assert.That(loadedRow["TheNumeric"], Is.TypeOf<decimal>());

            Assert.That(loadedRow["TheFloat"], Is.EqualTo(1.2f));
            Assert.That(loadedRow["TheFloat"], Is.TypeOf<float>());

            Assert.That(loadedRow["TheDouble"], Is.EqualTo(101.3d));
            Assert.That(loadedRow["TheDouble"], Is.TypeOf<double>());

            Assert.That(loadedRow["TheDate"], Is.EqualTo(DateTime.Parse("2010-01-02")));
            Assert.That(loadedRow["TheDate"], Is.TypeOf<DateTime>());

            Assert.That(loadedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2011-11-12T10:10:10")));
            Assert.That(loadedRow["TheDateTime"], Is.TypeOf<DateTime>());

            Assert.That(loadedRow["TheTimeStamp"], Is.EqualTo(DateTime.Parse("2015-03-07T05:06:33")).Within(TimeSpan.FromSeconds(1)));
            Assert.That(loadedRow["TheTimeStamp"], Is.TypeOf<DateTime>());

            Assert.That(loadedRow["TheTime"], Is.EqualTo(TimeSpan.Parse("03:03:03")));
            Assert.That(loadedRow["TheTime"], Is.TypeOf<TimeSpan>());

            Assert.That(loadedRow["TheYear"], Is.EqualTo(1917));
            Assert.That(loadedRow["TheYear"], Is.TypeOf<short>());

            Assert.That(loadedRow["TheChar"], Is.EqualTo("abc"));
            Assert.That(loadedRow["TheChar"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheVarChar"], Is.EqualTo("Андрей Коваленко"));
            Assert.That(loadedRow["TheVarChar"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheBinary"], Is.EqualTo(new byte[] { 0x10, 0x20, 0x33, 0, 0, 0, 0, 0, 0, 0 }));
            Assert.That(loadedRow["TheBinary"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheVarBinary"], Is.EqualTo(new byte[] { 0xff, 0xee, 0xbb }));
            Assert.That(loadedRow["TheVarBinary"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheTinyText"], Is.EqualTo("ze tiny text"));
            Assert.That(loadedRow["TheTinyText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheText"], Is.EqualTo("Зе текст"));
            Assert.That(loadedRow["TheText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheMediumText"], Is.EqualTo("ze medium text"));
            Assert.That(loadedRow["TheMediumText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheLongText"], Is.EqualTo("Зе лонг текст"));
            Assert.That(loadedRow["TheLongText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheTinyBlob"], Is.EqualTo(new byte[] { 0x10, 0x11, 0x12 }));
            Assert.That(loadedRow["TheTinyBlob"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheBlob"], Is.EqualTo(new byte[] { 0x20, 0x21, 0x22 }));
            Assert.That(loadedRow["TheBlob"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheMediumBlob"], Is.EqualTo(new byte[] { 0x30, 0x31, 0x32 }));
            Assert.That(loadedRow["TheMediumBlob"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheLongBlob"], Is.EqualTo(new byte[] { 0x40, 0x41, 0x42 }));
            Assert.That(loadedRow["TheLongBlob"], Is.TypeOf<byte[]>());
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

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            using var command = this.Connection.CreateCommand();

            for (var i = 0; i < rows.Length; i++)
            {
                var row = rows[i];

                var createTableSql = @"
CREATE TABLE `zeta`.`MyTab`(
    `Id` int NOT NULL PRIMARY KEY AUTO_INCREMENT,
    `Length` int NULL DEFAULT NULL,
    `Name` nvarchar(100) DEFAULT 'Polly')
";
                command.CommandText = createTableSql;
                command.ExecuteNonQuery();

                cruder.InsertRow("MyTab", row, x => false);
                var insertedRow = TestHelper.LoadRow(this.Connection, "MyTab", 1);
                insertedRows[i] = insertedRow;

                this.Connection.ExecuteSingleSql("DROP TABLE `zeta`.`MyTab`");
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

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            for (var i = 0; i < rows.Length; i++)
            {
                var row = rows[i];
                cruder.InsertRow("SmallTable", row, x => false);

                var lastIdentity = (int)this.Connection.GetLastIdentity();

                var insertedRow = TestHelper.LoadRow(
                    this.Connection,
                    "SmallTable",
                    lastIdentity);

                insertedRows[i] = insertedRow;

                this.Connection.ExecuteSingleSql("DELETE FROM `zeta`.`SmallTable`");
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

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            var row = new
            {
                TheInt = 1,
                TheNVarChar = "Polina",
                NotExisting = 100,
            };

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRow("SmallTable", row));

            // Assert
            Assert.That(ex, Has.Message.EqualTo($"Column 'NotExisting' does not exist."));
        }

        [Test]
        public void InsertRow_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            this.Connection.CreateSchema("bad_schema");

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("bad_schema");
            this.Connection.ExecuteSingleSql("CREATE TABLE bad_schema.some_table(id int PRIMARY KEY)");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            this.Connection.DropSchema("bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRow("some_table", new object()));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void InsertRow_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRow("bad_table", new object()));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void InsertRow_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.InsertRow(null, new object(), x => true));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void InsertRow_RowIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new MySqlCruder(this.Connection);

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

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);
            var row = new
            {
                TheInt = DBNull.Value,
            };

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRow("SuperTable", row, x => x == "TheInt"));

            // Assert
            Assert.That(ex,
                Has.Message.EqualTo(
                    "Could not transform value '' of type 'System.DBNull'. Table name is 'supertable'. Column name is 'TheInt'."));
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

            this.Connection.ExecuteSingleSql("ALTER TABLE `zeta`.`HealthInfo` DROP CONSTRAINT `FK_healthInfo_Person`");

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);
            cruder.GetTableValuesConverter("HealthInfo").SetColumnConverter("Id", new MySqlGuidValueConverter(MySqlGuidBehaviour.Char36));

            // Act
            cruder.InsertRows("HealthInfo", rows, x => x != "NotExisting");

            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    *
FROM
    `zeta`.`HealthInfo`
ORDER BY
    `Id`
";
            var loadedRows = command.GetCommandRows();
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

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            cruder.InsertRows("SmallTable", rows, x => false);

            // Assert
            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    *
FROM
    `zeta`.`SmallTable`
ORDER BY
    `Id`
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

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            cruder.InsertRows("SmallTable", rows, x => false);

            // Assert
            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    *
FROM
    `zeta`.`SmallTable`
ORDER BY
    `Id`
";

            var loadedRows = command.GetCommandRows();
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

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            cruder.InsertRows("SmallTable", rows);

            // Assert
            using var command = this.Connection.CreateCommand();
            command.CommandText = @"
SELECT
    *
FROM
    `zeta`.`SmallTable`
ORDER BY
    `Id`
";

            var loadedRows = command.GetCommandRows();
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

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

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

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => cruder.InsertRows("SmallTable", rows, x => true));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("'values' does not contain property representing column 'TheInt'."));
        }

        [Test]
        public void InsertRows_SchemaDoesNotExist_TauDbException()
        {
            // Arrange
            this.Connection.CreateSchema("bad_schema");

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("bad_schema");
            this.Connection.ExecuteSingleSql("CREATE TABLE bad_schema.some_table(id int PRIMARY KEY)");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            this.Connection.DropSchema("bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRows("some_table", new object[] { new { id = 1 } }));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void InsertRows_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRows("bad_table", new object[] { }));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void InsertRows_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.InsertRows(null, new object[] { }));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void InsertRows_RowsIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new MySqlCruder(this.Connection);

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

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

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

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.InsertRows("SmallTable", rows));

            // Assert
            Assert.That(ex,
                Has.Message.StartWith(
                    "Could not transform value '' of type 'System.DBNull'. Table name is 'smalltable'. Column name is 'TheInt'."));
        }

        #endregion

        #region RowInsertedCallback

        [Test]
        public void RowInsertedCallback_SetToSomeValue_KeepsThatValue()
        {
            // Arrange
            this.CreateSmallTable();

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);
            var sb1 = new StringBuilder();

            // Act
            Action<string, object, int> callback = (tableName, row, index) =>
            {
                sb1.Append($"Table name: {tableName}; index: {index}");
            };

            cruder.RowInsertedCallback = callback;

            cruder.InsertRow("SmallTable", new object());
            var callback1 = cruder.RowInsertedCallback;

            cruder.RowInsertedCallback = null;
            var callback2 = cruder.RowInsertedCallback;

            // Assert
            var s = sb1.ToString();
            Assert.That(s, Is.EqualTo("Table name: SmallTable; index: 0"));

            Assert.That(callback1, Is.SameAs(callback));
            Assert.That(callback2, Is.Null);
        }

        [Test]
        public void RowInsertedCallback_SetToNonNull_IsCalledWhenInsertRowIsCalled()
        {
            // Arrange
            this.CreateSmallTable();

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);
            var sb1 = new StringBuilder();

            // Act
            cruder.RowInsertedCallback = (tableName, row, index) =>
            {
                sb1.Append($"Table name: {tableName}; index: {index}");
            };

            cruder.InsertRow("SmallTable", new object());

            // Assert
            var s = sb1.ToString();
            Assert.That(s, Is.EqualTo("Table name: SmallTable; index: 0"));
        }

        [Test]
        public void RowInsertedCallback_SetToNonNull_IsCalledWhenInsertRowsIsCalled()
        {
            // Arrange
            this.CreateSmallTable();

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);
            var sb1 = new StringBuilder();

            // Act
            cruder.RowInsertedCallback = (tableName, row, index) =>
            {
                sb1.AppendLine($"Table name: {tableName}; index: {index}; int: {((dynamic)row).TheInt}");
            };

            cruder.InsertRows(
                "SmallTable",
                new object[]
                {
                    new
                    {
                        TheInt = 11,
                    },
                    new
                    {
                        TheInt = 22,
                    },
                });

            // Assert
            var s = sb1.ToString();
            Assert.That(s, Is.EqualTo(@"Table name: SmallTable; index: 0; int: 11
Table name: SmallTable; index: 1; int: 22
"));
        }

        #endregion

        #region GetRow

        [Test]
        public void GetRow_ValidArguments_ReturnsRow()
        {
            // Arrange
            this.CreateSuperTable();

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            dynamic row = new DynamicRow(this.CreateSuperTableRowDto());
            row.DeleteValue("NotExisting");

            cruder.InsertRow("SuperTable", row, (Func<string, bool>)(x => true)); // InsertRow is ut'ed already :)

            // Act
            var loadedRow = ((DynamicRow)cruder.GetRow("SuperTable", -13, x => x.Contains("Date"))).ToDictionary();

            // Assert
            Assert.That(loadedRow.Count, Is.EqualTo(2));

            Assert.That(loadedRow["TheDate"], Is.EqualTo(DateTime.Parse("2010-01-02")));
            Assert.That(loadedRow["TheDate"], Is.TypeOf<DateTime>());

            Assert.That(loadedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2011-11-12T10:10:10")));
            Assert.That(loadedRow["TheDateTime"], Is.TypeOf<DateTime>());
        }

        [Test]
        public void GetRow_AllDataTypes_RunsOk()
        {
            // Arrange
            this.CreateSuperTable();

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            this.InsertSuperTableRow();

            // Act
            var loadedRow = ((DynamicRow)cruder.GetRow("SuperTable", -13)).ToDictionary();

            // Assert
            Assert.That(loadedRow["TheInt"], Is.EqualTo(-13));
            Assert.That(loadedRow["TheInt"], Is.TypeOf<int>());

            Assert.That(loadedRow["TheIntUnsigned"], Is.EqualTo(13));
            Assert.That(loadedRow["TheIntUnsigned"], Is.TypeOf<uint>());

            Assert.That(loadedRow["TheBit"], Is.EqualTo(1));
            Assert.That(loadedRow["TheBit"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheBit9"], Is.EqualTo(257));
            Assert.That(loadedRow["TheBit9"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheBit17"], Is.EqualTo(65536));
            Assert.That(loadedRow["TheBit17"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheBit33"], Is.EqualTo((ulong)int.MaxValue + 10));
            Assert.That(loadedRow["TheBit33"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheTinyInt"], Is.EqualTo(-17));
            Assert.That(loadedRow["TheTinyInt"], Is.TypeOf<sbyte>());

            Assert.That(loadedRow["TheTinyIntUnsigned"], Is.EqualTo(17));
            Assert.That(loadedRow["TheTinyIntUnsigned"], Is.TypeOf<byte>());

            Assert.That(loadedRow["TheBool"], Is.EqualTo(0));
            Assert.That(loadedRow["TheBool"], Is.TypeOf<sbyte>());

            Assert.That(loadedRow["TheBoolean"], Is.EqualTo(1));
            Assert.That(loadedRow["TheBoolean"], Is.TypeOf<sbyte>());

            Assert.That(loadedRow["TheSmallInt"], Is.EqualTo(-777));
            Assert.That(loadedRow["TheSmallInt"], Is.TypeOf<short>());

            Assert.That(loadedRow["TheSmallIntUnsigned"], Is.EqualTo(777));
            Assert.That(loadedRow["TheSmallIntUnsigned"], Is.TypeOf<ushort>());

            Assert.That(loadedRow["TheMediumInt"], Is.EqualTo(-65536));
            Assert.That(loadedRow["TheMediumInt"], Is.TypeOf<int>());

            Assert.That(loadedRow["TheMediumIntUnsigned"], Is.EqualTo(65536));
            Assert.That(loadedRow["TheMediumIntUnsigned"], Is.TypeOf<uint>());

            Assert.That(loadedRow["TheBigInt"], Is.EqualTo((long)int.MinValue - 1));
            Assert.That(loadedRow["TheBigInt"], Is.TypeOf<long>());

            Assert.That(loadedRow["TheBigIntUnsigned"], Is.EqualTo((ulong)int.MaxValue + 1));
            Assert.That(loadedRow["TheBigIntUnsigned"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheDecimal"], Is.EqualTo(11.2m));
            Assert.That(loadedRow["TheDecimal"], Is.TypeOf<decimal>());

            Assert.That(loadedRow["TheNumeric"], Is.EqualTo(22.3m));
            Assert.That(loadedRow["TheNumeric"], Is.TypeOf<decimal>());

            Assert.That(loadedRow["TheFloat"], Is.EqualTo(1.2f));
            Assert.That(loadedRow["TheFloat"], Is.TypeOf<float>());

            Assert.That(loadedRow["TheDouble"], Is.EqualTo(101.3d));
            Assert.That(loadedRow["TheDouble"], Is.TypeOf<double>());

            Assert.That(loadedRow["TheDate"], Is.EqualTo(DateTime.Parse("2010-01-02")));
            Assert.That(loadedRow["TheDate"], Is.TypeOf<DateTime>());

            Assert.That(loadedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2011-11-12T10:10:10")));
            Assert.That(loadedRow["TheDateTime"], Is.TypeOf<DateTime>());

            Assert.That(loadedRow["TheTimeStamp"], Is.EqualTo(DateTime.Parse("2015-03-07T05:06:33")).Within(TimeSpan.FromSeconds(1)));
            Assert.That(loadedRow["TheTimeStamp"], Is.TypeOf<DateTime>());

            Assert.That(loadedRow["TheTime"], Is.EqualTo(TimeSpan.Parse("03:03:03")));
            Assert.That(loadedRow["TheTime"], Is.TypeOf<TimeSpan>());

            Assert.That(loadedRow["TheYear"], Is.EqualTo(1917));
            Assert.That(loadedRow["TheYear"], Is.TypeOf<short>());

            Assert.That(loadedRow["TheChar"], Is.EqualTo("abc"));
            Assert.That(loadedRow["TheChar"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheVarChar"], Is.EqualTo("Андрей Коваленко"));
            Assert.That(loadedRow["TheVarChar"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheBinary"], Is.EqualTo(new byte[] { 0x10, 0x20, 0x33, 0, 0, 0, 0, 0, 0, 0 }));
            Assert.That(loadedRow["TheBinary"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheVarBinary"], Is.EqualTo(new byte[] { 0xff, 0xee, 0xbb }));
            Assert.That(loadedRow["TheVarBinary"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheTinyText"], Is.EqualTo("ze tiny text"));
            Assert.That(loadedRow["TheTinyText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheText"], Is.EqualTo("Зе текст"));
            Assert.That(loadedRow["TheText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheMediumText"], Is.EqualTo("ze medium text"));
            Assert.That(loadedRow["TheMediumText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheLongText"], Is.EqualTo("Зе лонг текст"));
            Assert.That(loadedRow["TheLongText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheTinyBlob"], Is.EqualTo(new byte[] { 0x10, 0x11, 0x12 }));
            Assert.That(loadedRow["TheTinyBlob"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheBlob"], Is.EqualTo(new byte[] { 0x20, 0x21, 0x22 }));
            Assert.That(loadedRow["TheBlob"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheMediumBlob"], Is.EqualTo(new byte[] { 0x30, 0x31, 0x32 }));
            Assert.That(loadedRow["TheMediumBlob"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheLongBlob"], Is.EqualTo(new byte[] { 0x40, 0x41, 0x42 }));
            Assert.That(loadedRow["TheLongBlob"], Is.TypeOf<byte[]>());
        }

        [Test]
        public void GetRow_SelectorIsTruer_DeliversAllColumns()
        {
            // Arrange
            this.CreateSuperTable();

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            this.InsertSuperTableRow();

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var loadedRow = ((DynamicRow)cruder.GetRow("SuperTable", -13, x => true)).ToDictionary();

            // Assert
            Assert.That(loadedRow["TheInt"], Is.EqualTo(-13));
            Assert.That(loadedRow["TheInt"], Is.TypeOf<int>());

            Assert.That(loadedRow["TheIntUnsigned"], Is.EqualTo(13));
            Assert.That(loadedRow["TheIntUnsigned"], Is.TypeOf<uint>());

            Assert.That(loadedRow["TheBit"], Is.EqualTo(1));
            Assert.That(loadedRow["TheBit"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheBit9"], Is.EqualTo(257));
            Assert.That(loadedRow["TheBit9"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheBit17"], Is.EqualTo(65536));
            Assert.That(loadedRow["TheBit17"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheBit33"], Is.EqualTo((ulong)int.MaxValue + 10));
            Assert.That(loadedRow["TheBit33"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheTinyInt"], Is.EqualTo(-17));
            Assert.That(loadedRow["TheTinyInt"], Is.TypeOf<sbyte>());

            Assert.That(loadedRow["TheTinyIntUnsigned"], Is.EqualTo(17));
            Assert.That(loadedRow["TheTinyIntUnsigned"], Is.TypeOf<byte>());

            Assert.That(loadedRow["TheBool"], Is.EqualTo(0));
            Assert.That(loadedRow["TheBool"], Is.TypeOf<sbyte>());

            Assert.That(loadedRow["TheBoolean"], Is.EqualTo(1));
            Assert.That(loadedRow["TheBoolean"], Is.TypeOf<sbyte>());

            Assert.That(loadedRow["TheSmallInt"], Is.EqualTo(-777));
            Assert.That(loadedRow["TheSmallInt"], Is.TypeOf<short>());

            Assert.That(loadedRow["TheSmallIntUnsigned"], Is.EqualTo(777));
            Assert.That(loadedRow["TheSmallIntUnsigned"], Is.TypeOf<ushort>());

            Assert.That(loadedRow["TheMediumInt"], Is.EqualTo(-65536));
            Assert.That(loadedRow["TheMediumInt"], Is.TypeOf<int>());

            Assert.That(loadedRow["TheMediumIntUnsigned"], Is.EqualTo(65536));
            Assert.That(loadedRow["TheMediumIntUnsigned"], Is.TypeOf<uint>());

            Assert.That(loadedRow["TheBigInt"], Is.EqualTo((long)int.MinValue - 1));
            Assert.That(loadedRow["TheBigInt"], Is.TypeOf<long>());

            Assert.That(loadedRow["TheBigIntUnsigned"], Is.EqualTo((ulong)int.MaxValue + 1));
            Assert.That(loadedRow["TheBigIntUnsigned"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheDecimal"], Is.EqualTo(11.2m));
            Assert.That(loadedRow["TheDecimal"], Is.TypeOf<decimal>());

            Assert.That(loadedRow["TheNumeric"], Is.EqualTo(22.3m));
            Assert.That(loadedRow["TheNumeric"], Is.TypeOf<decimal>());

            Assert.That(loadedRow["TheFloat"], Is.EqualTo(1.2f));
            Assert.That(loadedRow["TheFloat"], Is.TypeOf<float>());

            Assert.That(loadedRow["TheDouble"], Is.EqualTo(101.3d));
            Assert.That(loadedRow["TheDouble"], Is.TypeOf<double>());

            Assert.That(loadedRow["TheDate"], Is.EqualTo(DateTime.Parse("2010-01-02")));
            Assert.That(loadedRow["TheDate"], Is.TypeOf<DateTime>());

            Assert.That(loadedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2011-11-12T10:10:10")));
            Assert.That(loadedRow["TheDateTime"], Is.TypeOf<DateTime>());

            Assert.That(loadedRow["TheTimeStamp"], Is.EqualTo(DateTime.Parse("2015-03-07T05:06:33")).Within(TimeSpan.FromSeconds(1)));
            Assert.That(loadedRow["TheTimeStamp"], Is.TypeOf<DateTime>());

            Assert.That(loadedRow["TheTime"], Is.EqualTo(TimeSpan.Parse("03:03:03")));
            Assert.That(loadedRow["TheTime"], Is.TypeOf<TimeSpan>());

            Assert.That(loadedRow["TheYear"], Is.EqualTo(1917));
            Assert.That(loadedRow["TheYear"], Is.TypeOf<short>());

            Assert.That(loadedRow["TheChar"], Is.EqualTo("abc"));
            Assert.That(loadedRow["TheChar"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheVarChar"], Is.EqualTo("Андрей Коваленко"));
            Assert.That(loadedRow["TheVarChar"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheBinary"], Is.EqualTo(new byte[] { 0x10, 0x20, 0x33, 0, 0, 0, 0, 0, 0, 0 }));
            Assert.That(loadedRow["TheBinary"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheVarBinary"], Is.EqualTo(new byte[] { 0xff, 0xee, 0xbb }));
            Assert.That(loadedRow["TheVarBinary"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheTinyText"], Is.EqualTo("ze tiny text"));
            Assert.That(loadedRow["TheTinyText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheText"], Is.EqualTo("Зе текст"));
            Assert.That(loadedRow["TheText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheMediumText"], Is.EqualTo("ze medium text"));
            Assert.That(loadedRow["TheMediumText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheLongText"], Is.EqualTo("Зе лонг текст"));
            Assert.That(loadedRow["TheLongText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheTinyBlob"], Is.EqualTo(new byte[] { 0x10, 0x11, 0x12 }));
            Assert.That(loadedRow["TheTinyBlob"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheBlob"], Is.EqualTo(new byte[] { 0x20, 0x21, 0x22 }));
            Assert.That(loadedRow["TheBlob"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheMediumBlob"], Is.EqualTo(new byte[] { 0x30, 0x31, 0x32 }));
            Assert.That(loadedRow["TheMediumBlob"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheLongBlob"], Is.EqualTo(new byte[] { 0x40, 0x41, 0x42 }));
            Assert.That(loadedRow["TheLongBlob"], Is.TypeOf<byte[]>());
        }

        [Test]
        public void GetRow_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            this.Connection.CreateSchema("bad_schema");

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("bad_schema");
            this.Connection.ExecuteSingleSql("CREATE TABLE bad_schema.some_table(id int PRIMARY KEY)");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            this.Connection.DropSchema("bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetRow("some_table", 1));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetRow_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetRow("bad_table", 1));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void GetRow_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.GetRow(null, 1));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void GetRow_IdIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.GetRow("some_table", null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("id"));
        }

        [Test]
        public void GetRow_TableHasNoPrimaryKey_ThrowsArgumentException()

        {
            // Arrange
            this.Connection.ExecuteSingleSql("CREATE TABLE `zeta`.`dummy`(Foo int)"); // no PK

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

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

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentException>((() => cruder.GetRow("Person", "the_id")));

            // Assert
            Assert.That(
                ex,
                Has.Message.StartsWith("Failed to retrieve single primary key column name for the table 'person'."));
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void GetRow_IdNotFound_ReturnsNull()
        {
            // Arrange
            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);
            const int nonExistingId = 133;

            // Act
            var row = cruder.GetRow("NumericData", nonExistingId);

            // Assert
            Assert.That(row, Is.Null);
        }

        [Test]
        public void GetRow_SelectorIsFalser_ThrowsArgumentException()
        {
            // Arrange
            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

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

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);
            cruder.GetTableValuesConverter("DateData").SetColumnConverter("Moment", new DateTimeOffsetValueConverter());

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

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);
            cruder.GetTableValuesConverter("DateData").SetColumnConverter("Id", new MySqlGuidValueConverter(MySqlGuidBehaviour.Char36));
            cruder.GetTableValuesConverter("DateData").SetColumnConverter("Moment", new DateTimeOffsetValueConverter());

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
            this.Connection.CreateSchema("bad_schema");

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("bad_schema");
            this.Connection.ExecuteSingleSql("CREATE TABLE some_table(id int PRIMARY KEY)");

            IDbCruder cruder = new MySqlCruder(this.Connection);
            this.Connection.DropSchema("bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetAllRows("some_table"));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetAllRows_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.GetAllRows("bad_table"));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void GetAllRows_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.GetAllRows(null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void GetAllRows_SelectorIsFalser_ThrowsArgumentException()
        {
            // Arrange
            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

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
            var id = -13; // will be inserted by IDENTITY

            var update1 = new Dictionary<string, object>
            {
                {"TheInt", id},
                {"TheTinyInt", (byte) 2},
                {"TheSmallInt", (short) 22},
                {"TheBigInt", 2222L},
                {"NotExisting", 777},
            };

            var update2 = new DynamicRow();
            update2.SetValue("TheInt", id);
            update2.SetValue("TheTinyInt", (byte)2);
            update2.SetValue("TheSmallInt", (short)22);
            update2.SetValue("TheBigInt", 2222L);
            update2.SetValue("NotExisting", 777);

            var update3 = new
            {
                TheInt = id,
                TheTinyInt = (byte)2,
                TheSmallInt = (short)22,
                TheBigInt = 2222L,
                NotExisting = 777,
            };

            var update4 = new SuperTableRowDto
            {
                TheInt = id,
                TheTinyInt = 2,
                TheSmallInt = 22,
                TheBigInt = 2222L,
                NotExisting = 777,
            };

            var updates = new object[]
            {
                update1,
                update2,
                update3,
                update4,
            };

            var loadedRows = new IReadOnlyDictionary<string, object>[updates.Length];

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            for (var i = 0; i < updates.Length; i++)
            {
                this.CreateSuperTable();
                this.InsertSuperTableRow();

                cruder.UpdateRow(
                    "SuperTable",
                    updates[i],
                    x => x.IsIn(
                        "Id",
                        "TheTinyInt",
                        "TheSmallInt",
                        "TheInt",
                        "TheBigInt"));

                var loadedRow = TestHelper.LoadRow(this.Connection, "SuperTable", -13);
                loadedRows[i] = loadedRow;

                this.Connection.ExecuteSingleSql("DROP TABLE `zeta`.`SuperTable`");
            }

            for (var i = 0; i < loadedRows.Length; i++)
            {
                var loadedRow = loadedRows[i];

                Assert.That(loadedRow["TheInt"], Is.EqualTo(-13));
                Assert.That(loadedRow["TheInt"], Is.TypeOf<int>());

                Assert.That(loadedRow["TheIntUnsigned"], Is.EqualTo(13));
                Assert.That(loadedRow["TheIntUnsigned"], Is.TypeOf<uint>());

                Assert.That(loadedRow["TheBit"], Is.EqualTo(1));
                Assert.That(loadedRow["TheBit"], Is.TypeOf<ulong>());

                Assert.That(loadedRow["TheBit9"], Is.EqualTo(257));
                Assert.That(loadedRow["TheBit9"], Is.TypeOf<ulong>());

                Assert.That(loadedRow["TheBit17"], Is.EqualTo(65536));
                Assert.That(loadedRow["TheBit17"], Is.TypeOf<ulong>());

                Assert.That(loadedRow["TheBit33"], Is.EqualTo((ulong)int.MaxValue + 10));
                Assert.That(loadedRow["TheBit33"], Is.TypeOf<ulong>());

                Assert.That(loadedRow["TheTinyInt"], Is.EqualTo(2));
                Assert.That(loadedRow["TheTinyInt"], Is.TypeOf<sbyte>());

                Assert.That(loadedRow["TheTinyIntUnsigned"], Is.EqualTo(17));
                Assert.That(loadedRow["TheTinyIntUnsigned"], Is.TypeOf<byte>());

                Assert.That(loadedRow["TheBool"], Is.EqualTo(false));
                Assert.That(loadedRow["TheBool"], Is.TypeOf<bool>());

                Assert.That(loadedRow["TheBoolean"], Is.EqualTo(true));
                Assert.That(loadedRow["TheBoolean"], Is.TypeOf<bool>());

                Assert.That(loadedRow["TheSmallInt"], Is.EqualTo(22));
                Assert.That(loadedRow["TheSmallInt"], Is.TypeOf<short>());

                Assert.That(loadedRow["TheSmallIntUnsigned"], Is.EqualTo(777));
                Assert.That(loadedRow["TheSmallIntUnsigned"], Is.TypeOf<ushort>());

                Assert.That(loadedRow["TheMediumInt"], Is.EqualTo(-65536));
                Assert.That(loadedRow["TheMediumInt"], Is.TypeOf<int>());

                Assert.That(loadedRow["TheMediumIntUnsigned"], Is.EqualTo(65536));
                Assert.That(loadedRow["TheMediumIntUnsigned"], Is.TypeOf<uint>());

                Assert.That(loadedRow["TheBigInt"], Is.EqualTo(2222));
                Assert.That(loadedRow["TheBigInt"], Is.TypeOf<long>());

                Assert.That(loadedRow["TheBigIntUnsigned"], Is.EqualTo((ulong)int.MaxValue + 1));
                Assert.That(loadedRow["TheBigIntUnsigned"], Is.TypeOf<ulong>());

                Assert.That(loadedRow["TheDecimal"], Is.EqualTo(11.2m));
                Assert.That(loadedRow["TheDecimal"], Is.TypeOf<decimal>());

                Assert.That(loadedRow["TheNumeric"], Is.EqualTo(22.3m));
                Assert.That(loadedRow["TheNumeric"], Is.TypeOf<decimal>());

                Assert.That(loadedRow["TheFloat"], Is.EqualTo(1.2f));
                Assert.That(loadedRow["TheFloat"], Is.TypeOf<float>());

                Assert.That(loadedRow["TheDouble"], Is.EqualTo(101.3d));
                Assert.That(loadedRow["TheDouble"], Is.TypeOf<double>());

                Assert.That(loadedRow["TheDate"], Is.EqualTo(DateTime.Parse("2010-01-02")));
                Assert.That(loadedRow["TheDate"], Is.TypeOf<DateTime>());

                Assert.That(loadedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2011-11-12T10:10:10")));
                Assert.That(loadedRow["TheDateTime"], Is.TypeOf<DateTime>());

                Assert.That(loadedRow["TheTimeStamp"], Is.EqualTo(DateTime.Parse("2015-03-07T05:06:33")).Within(TimeSpan.FromSeconds(1)));
                Assert.That(loadedRow["TheTimeStamp"], Is.TypeOf<DateTime>());

                Assert.That(loadedRow["TheTime"], Is.EqualTo(TimeSpan.Parse("03:03:03")));
                Assert.That(loadedRow["TheTime"], Is.TypeOf<TimeSpan>());

                Assert.That(loadedRow["TheYear"], Is.EqualTo(1917));
                Assert.That(loadedRow["TheYear"], Is.TypeOf<short>());

                Assert.That(loadedRow["TheChar"], Is.EqualTo("abc"));
                Assert.That(loadedRow["TheChar"], Is.TypeOf<string>());

                Assert.That(loadedRow["TheVarChar"], Is.EqualTo("Андрей Коваленко"));
                Assert.That(loadedRow["TheVarChar"], Is.TypeOf<string>());

                Assert.That(loadedRow["TheBinary"], Is.EqualTo(new byte[] { 0x10, 0x20, 0x33, 0, 0, 0, 0, 0, 0, 0 }));
                Assert.That(loadedRow["TheBinary"], Is.TypeOf<byte[]>());

                Assert.That(loadedRow["TheVarBinary"], Is.EqualTo(new byte[] { 0xff, 0xee, 0xbb }));
                Assert.That(loadedRow["TheVarBinary"], Is.TypeOf<byte[]>());

                Assert.That(loadedRow["TheTinyText"], Is.EqualTo("ze tiny text"));
                Assert.That(loadedRow["TheTinyText"], Is.TypeOf<string>());

                Assert.That(loadedRow["TheText"], Is.EqualTo("Зе текст"));
                Assert.That(loadedRow["TheText"], Is.TypeOf<string>());

                Assert.That(loadedRow["TheMediumText"], Is.EqualTo("ze medium text"));
                Assert.That(loadedRow["TheMediumText"], Is.TypeOf<string>());

                Assert.That(loadedRow["TheLongText"], Is.EqualTo("Зе лонг текст"));
                Assert.That(loadedRow["TheLongText"], Is.TypeOf<string>());

                Assert.That(loadedRow["TheTinyBlob"], Is.EqualTo(new byte[] { 0x10, 0x11, 0x12 }));
                Assert.That(loadedRow["TheTinyBlob"], Is.TypeOf<byte[]>());

                Assert.That(loadedRow["TheBlob"], Is.EqualTo(new byte[] { 0x20, 0x21, 0x22 }));
                Assert.That(loadedRow["TheBlob"], Is.TypeOf<byte[]>());

                Assert.That(loadedRow["TheMediumBlob"], Is.EqualTo(new byte[] { 0x30, 0x31, 0x32 }));
                Assert.That(loadedRow["TheMediumBlob"], Is.TypeOf<byte[]>());

                Assert.That(loadedRow["TheLongBlob"], Is.EqualTo(new byte[] { 0x40, 0x41, 0x42 }));
                Assert.That(loadedRow["TheLongBlob"], Is.TypeOf<byte[]>());
            }
        }

        [Test]
        public void UpdateRow_AllDataTypes_RunsOk()
        {
            // Arrange
            this.CreateSuperTable();

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            this.InsertSuperTableRow();

            var update = new SuperTableRowDto
            {
                TheInt = -13,
                TheIntUnsigned = 133,

                TheBit = 0,
                TheBit9 = 511,
                TheBit17 = 131071,
                TheBit33 = (ulong)int.MaxValue + 11,

                TheTinyInt = -18,
                TheTinyIntUnsigned = 18,

                TheBool = 1,
                TheBoolean = 0,

                TheSmallInt = -778,
                TheSmallIntUnsigned = 778,

                TheMediumInt = -65537,
                TheMediumIntUnsigned = 65537,

                TheBigInt = (long)int.MinValue - 2,
                TheBigIntUnsigned = (ulong)int.MaxValue + 2,

                TheDecimal = 11.3m,
                TheNumeric = 22.4m,

                TheFloat = 1.3f,
                TheDouble = 102.4d,

                TheDate = DateTime.Parse("2010-01-03"),
                TheDateTime = DateTime.Parse("2013-11-12T10:10:10"),
                TheTimeStamp = DateTime.Parse("2017-03-07T05:06:33.777"),
                TheTime = TimeSpan.Parse("03:03:08"),
                TheYear = 1921,

                TheChar = "def",
                TheVarChar = "Оливер Кромвель",

                TheBinary = new byte[] { 0x11, 0x21, 0x34 },
                TheVarBinary = new byte[] { 0xfa, 0xeb, 0xb3 },

                TheTinyText = "tiny string",
                TheText = "строка",
                TheMediumText = "medium string",
                TheLongText = "лонг строка",

                TheTinyBlob = new byte[] { 0xa0, 0xa1, 0xa2 },
                TheBlob = new byte[] { 0xb0, 0xb1, 0xb2 },
                TheMediumBlob = new byte[] { 0xc0, 0xc1, 0xc2 },
                TheLongBlob = new byte[] { 0xd0, 0xd1, 0xd2 },

                NotExisting = 777,
            };

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            cruder.UpdateRow("SuperTable", update, x => x != "NotExisting");

            // Assert
            var loadedRow = TestHelper.LoadRow(this.Connection, "SuperTable", -13);

            Assert.That(loadedRow["TheInt"], Is.EqualTo(-13));
            Assert.That(loadedRow["TheInt"], Is.TypeOf<int>());

            Assert.That(loadedRow["TheIntUnsigned"], Is.EqualTo(133));
            Assert.That(loadedRow["TheIntUnsigned"], Is.TypeOf<uint>());

            Assert.That(loadedRow["TheBit"], Is.EqualTo(0));
            Assert.That(loadedRow["TheBit"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheBit9"], Is.EqualTo(511));
            Assert.That(loadedRow["TheBit9"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheBit17"], Is.EqualTo(131071));
            Assert.That(loadedRow["TheBit17"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheBit33"], Is.EqualTo((ulong)int.MaxValue + 11));
            Assert.That(loadedRow["TheBit33"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheTinyInt"], Is.EqualTo(-18));
            Assert.That(loadedRow["TheTinyInt"], Is.TypeOf<sbyte>());

            Assert.That(loadedRow["TheTinyIntUnsigned"], Is.EqualTo(18));
            Assert.That(loadedRow["TheTinyIntUnsigned"], Is.TypeOf<byte>());

            Assert.That(loadedRow["TheBool"], Is.EqualTo(true));
            Assert.That(loadedRow["TheBool"], Is.TypeOf<bool>());

            Assert.That(loadedRow["TheBoolean"], Is.EqualTo(false));
            Assert.That(loadedRow["TheBoolean"], Is.TypeOf<bool>());

            Assert.That(loadedRow["TheSmallInt"], Is.EqualTo(-778));
            Assert.That(loadedRow["TheSmallInt"], Is.TypeOf<short>());

            Assert.That(loadedRow["TheSmallIntUnsigned"], Is.EqualTo(778));
            Assert.That(loadedRow["TheSmallIntUnsigned"], Is.TypeOf<ushort>());

            Assert.That(loadedRow["TheMediumInt"], Is.EqualTo(-65537));
            Assert.That(loadedRow["TheMediumInt"], Is.TypeOf<int>());

            Assert.That(loadedRow["TheMediumIntUnsigned"], Is.EqualTo(65537));
            Assert.That(loadedRow["TheMediumIntUnsigned"], Is.TypeOf<uint>());

            Assert.That(loadedRow["TheBigInt"], Is.EqualTo((long)int.MinValue - 2));
            Assert.That(loadedRow["TheBigInt"], Is.TypeOf<long>());

            Assert.That(loadedRow["TheBigIntUnsigned"], Is.EqualTo((ulong)int.MaxValue + 2));
            Assert.That(loadedRow["TheBigIntUnsigned"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheDecimal"], Is.EqualTo(11.3m));
            Assert.That(loadedRow["TheDecimal"], Is.TypeOf<decimal>());

            Assert.That(loadedRow["TheNumeric"], Is.EqualTo(22.4m));
            Assert.That(loadedRow["TheNumeric"], Is.TypeOf<decimal>());

            Assert.That(loadedRow["TheFloat"], Is.EqualTo(1.3f));
            Assert.That(loadedRow["TheFloat"], Is.TypeOf<float>());

            Assert.That(loadedRow["TheDouble"], Is.EqualTo(102.4d));
            Assert.That(loadedRow["TheDouble"], Is.TypeOf<double>());

            Assert.That(loadedRow["TheDate"], Is.EqualTo(DateTime.Parse("2010-01-03")));
            Assert.That(loadedRow["TheDate"], Is.TypeOf<DateTime>());

            Assert.That(loadedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2013-11-12T10:10:10")));
            Assert.That(loadedRow["TheDateTime"], Is.TypeOf<DateTime>());

            Assert.That(loadedRow["TheTimeStamp"], Is.EqualTo(DateTime.Parse("2017-03-07T05:06:33.777")).Within(TimeSpan.FromSeconds(1)));
            Assert.That(loadedRow["TheTimeStamp"], Is.TypeOf<DateTime>());

            Assert.That(loadedRow["TheTime"], Is.EqualTo(TimeSpan.Parse("03:03:08")));
            Assert.That(loadedRow["TheTime"], Is.TypeOf<TimeSpan>());

            Assert.That(loadedRow["TheYear"], Is.EqualTo(1921));
            Assert.That(loadedRow["TheYear"], Is.TypeOf<short>());

            Assert.That(loadedRow["TheChar"], Is.EqualTo("def"));
            Assert.That(loadedRow["TheChar"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheVarChar"], Is.EqualTo("Оливер Кромвель"));
            Assert.That(loadedRow["TheVarChar"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheBinary"], Is.EqualTo(new byte[] { 0x11, 0x21, 0x34, 0, 0, 0, 0, 0, 0, 0 }));
            Assert.That(loadedRow["TheBinary"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheVarBinary"], Is.EqualTo(new byte[] { 0xfa, 0xeb, 0xb3 }));
            Assert.That(loadedRow["TheVarBinary"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheTinyText"], Is.EqualTo("tiny string"));
            Assert.That(loadedRow["TheTinyText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheText"], Is.EqualTo("строка"));
            Assert.That(loadedRow["TheText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheMediumText"], Is.EqualTo("medium string"));
            Assert.That(loadedRow["TheMediumText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheLongText"], Is.EqualTo("лонг строка"));
            Assert.That(loadedRow["TheLongText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheTinyBlob"], Is.EqualTo(new byte[] { 0xa0, 0xa1, 0xa2 }));
            Assert.That(loadedRow["TheTinyBlob"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheBlob"], Is.EqualTo(new byte[] { 0xb0, 0xb1, 0xb2 }));
            Assert.That(loadedRow["TheBlob"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheMediumBlob"], Is.EqualTo(new byte[] { 0xc0, 0xc1, 0xc2 }));
            Assert.That(loadedRow["TheMediumBlob"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheLongBlob"], Is.EqualTo(new byte[] { 0xd0, 0xd1, 0xd2 }));
            Assert.That(loadedRow["TheLongBlob"], Is.TypeOf<byte[]>());
        }

        [Test]
        public void UpdateRow_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            this.Connection.CreateSchema("bad_schema");

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("bad_schema");
            this.Connection.ExecuteSingleSql("CREATE TABLE bad_schema.some_table(id int PRIMARY KEY, name varchar(10))");

            IDbCruder cruder = new MySqlCruder(this.Connection);
            
            this.Connection.DropSchema("bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.UpdateRow("some_table", new { id = 1, name = "a" }));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void UpdateRow_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.UpdateRow("bad_table", new { Id = 1, Name = 2 }));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void UpdateRow_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() =>
                cruder.UpdateRow(null, new { Id = 1, Name = 2 }, x => true));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void UpdateRow_RowUpdateIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() =>
                cruder.UpdateRow("SuperTable", null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("rowUpdate"));
        }

        [Test]
        public void UpdateRow_PropertySelectorIsNull_UsesAllProperties()
        {
            // Arrange
            this.CreateSuperTable();
            this.InsertSuperTableRow();

            var updateMold = new SuperTableRowDto
            {
                TheInt = -13,
                TheIntUnsigned = 133,

                TheBit = 0,
                TheBit9 = 511,
                TheBit17 = 131071,
                TheBit33 = (ulong)int.MaxValue + 11,

                TheTinyInt = -18,
                TheTinyIntUnsigned = 18,

                TheBool = 1,
                TheBoolean = 0,

                TheSmallInt = -778,
                TheSmallIntUnsigned = 778,

                TheMediumInt = -65537,
                TheMediumIntUnsigned = 65537,

                TheBigInt = (long)int.MinValue - 2,
                TheBigIntUnsigned = (ulong)int.MaxValue + 2,

                TheDecimal = 11.3m,
                TheNumeric = 22.4m,

                TheFloat = 1.3f,
                TheDouble = 102.4d,

                TheDate = DateTime.Parse("2010-01-03"),
                TheDateTime = DateTime.Parse("2013-11-12T10:10:10"),
                TheTimeStamp = DateTime.Parse("2017-03-07T05:06:33.777"),
                TheTime = TimeSpan.Parse("03:03:08"),
                TheYear = 1921,

                TheChar = "def",
                TheVarChar = "Оливер Кромвель",

                TheBinary = new byte[] { 0x11, 0x21, 0x34 },
                TheVarBinary = new byte[] { 0xfa, 0xeb, 0xb3 },

                TheTinyText = "tiny string",
                TheText = "строка",
                TheMediumText = "medium string",
                TheLongText = "лонг строка",

                TheTinyBlob = new byte[] { 0xa0, 0xa1, 0xa2 },
                TheBlob = new byte[] { 0xb0, 0xb1, 0xb2 },
                TheMediumBlob = new byte[] { 0xc0, 0xc1, 0xc2 },
                TheLongBlob = new byte[] { 0xd0, 0xd1, 0xd2 },
            };

            var update = new DynamicRow(updateMold);
            update.DeleteValue("NotExisting");

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            cruder.UpdateRow("SuperTable", update, null);

            // Assert
            var loadedRow = TestHelper.LoadRow(this.Connection, "SuperTable", -13);

            Assert.That(loadedRow["TheInt"], Is.EqualTo(-13));
            Assert.That(loadedRow["TheInt"], Is.TypeOf<int>());

            Assert.That(loadedRow["TheIntUnsigned"], Is.EqualTo(133));
            Assert.That(loadedRow["TheIntUnsigned"], Is.TypeOf<uint>());

            Assert.That(loadedRow["TheBit"], Is.EqualTo(0));
            Assert.That(loadedRow["TheBit"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheBit9"], Is.EqualTo(511));
            Assert.That(loadedRow["TheBit9"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheBit17"], Is.EqualTo(131071));
            Assert.That(loadedRow["TheBit17"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheBit33"], Is.EqualTo((ulong)int.MaxValue + 11));
            Assert.That(loadedRow["TheBit33"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheTinyInt"], Is.EqualTo(-18));
            Assert.That(loadedRow["TheTinyInt"], Is.TypeOf<sbyte>());

            Assert.That(loadedRow["TheTinyIntUnsigned"], Is.EqualTo(18));
            Assert.That(loadedRow["TheTinyIntUnsigned"], Is.TypeOf<byte>());

            Assert.That(loadedRow["TheBool"], Is.EqualTo(true));
            Assert.That(loadedRow["TheBool"], Is.TypeOf<bool>());

            Assert.That(loadedRow["TheBoolean"], Is.EqualTo(false));
            Assert.That(loadedRow["TheBoolean"], Is.TypeOf<bool>());

            Assert.That(loadedRow["TheSmallInt"], Is.EqualTo(-778));
            Assert.That(loadedRow["TheSmallInt"], Is.TypeOf<short>());

            Assert.That(loadedRow["TheSmallIntUnsigned"], Is.EqualTo(778));
            Assert.That(loadedRow["TheSmallIntUnsigned"], Is.TypeOf<ushort>());

            Assert.That(loadedRow["TheMediumInt"], Is.EqualTo(-65537));
            Assert.That(loadedRow["TheMediumInt"], Is.TypeOf<int>());

            Assert.That(loadedRow["TheMediumIntUnsigned"], Is.EqualTo(65537));
            Assert.That(loadedRow["TheMediumIntUnsigned"], Is.TypeOf<uint>());

            Assert.That(loadedRow["TheBigInt"], Is.EqualTo((long)int.MinValue - 2));
            Assert.That(loadedRow["TheBigInt"], Is.TypeOf<long>());

            Assert.That(loadedRow["TheBigIntUnsigned"], Is.EqualTo((ulong)int.MaxValue + 2));
            Assert.That(loadedRow["TheBigIntUnsigned"], Is.TypeOf<ulong>());

            Assert.That(loadedRow["TheDecimal"], Is.EqualTo(11.3m));
            Assert.That(loadedRow["TheDecimal"], Is.TypeOf<decimal>());

            Assert.That(loadedRow["TheNumeric"], Is.EqualTo(22.4m));
            Assert.That(loadedRow["TheNumeric"], Is.TypeOf<decimal>());

            Assert.That(loadedRow["TheFloat"], Is.EqualTo(1.3f));
            Assert.That(loadedRow["TheFloat"], Is.TypeOf<float>());

            Assert.That(loadedRow["TheDouble"], Is.EqualTo(102.4d));
            Assert.That(loadedRow["TheDouble"], Is.TypeOf<double>());

            Assert.That(loadedRow["TheDate"], Is.EqualTo(DateTime.Parse("2010-01-03")));
            Assert.That(loadedRow["TheDate"], Is.TypeOf<DateTime>());

            Assert.That(loadedRow["TheDateTime"], Is.EqualTo(DateTime.Parse("2013-11-12T10:10:10")));
            Assert.That(loadedRow["TheDateTime"], Is.TypeOf<DateTime>());

            Assert.That(loadedRow["TheTimeStamp"], Is.EqualTo(DateTime.Parse("2017-03-07T05:06:33.777")).Within(TimeSpan.FromSeconds(1)));
            Assert.That(loadedRow["TheTimeStamp"], Is.TypeOf<DateTime>());

            Assert.That(loadedRow["TheTime"], Is.EqualTo(TimeSpan.Parse("03:03:08")));
            Assert.That(loadedRow["TheTime"], Is.TypeOf<TimeSpan>());

            Assert.That(loadedRow["TheYear"], Is.EqualTo(1921));
            Assert.That(loadedRow["TheYear"], Is.TypeOf<short>());

            Assert.That(loadedRow["TheChar"], Is.EqualTo("def"));
            Assert.That(loadedRow["TheChar"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheVarChar"], Is.EqualTo("Оливер Кромвель"));
            Assert.That(loadedRow["TheVarChar"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheBinary"], Is.EqualTo(new byte[] { 0x11, 0x21, 0x34, 0, 0, 0, 0, 0, 0, 0 }));
            Assert.That(loadedRow["TheBinary"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheVarBinary"], Is.EqualTo(new byte[] { 0xfa, 0xeb, 0xb3 }));
            Assert.That(loadedRow["TheVarBinary"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheTinyText"], Is.EqualTo("tiny string"));
            Assert.That(loadedRow["TheTinyText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheText"], Is.EqualTo("строка"));
            Assert.That(loadedRow["TheText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheMediumText"], Is.EqualTo("medium string"));
            Assert.That(loadedRow["TheMediumText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheLongText"], Is.EqualTo("лонг строка"));
            Assert.That(loadedRow["TheLongText"], Is.TypeOf<string>());

            Assert.That(loadedRow["TheTinyBlob"], Is.EqualTo(new byte[] { 0xa0, 0xa1, 0xa2 }));
            Assert.That(loadedRow["TheTinyBlob"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheBlob"], Is.EqualTo(new byte[] { 0xb0, 0xb1, 0xb2 }));
            Assert.That(loadedRow["TheBlob"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheMediumBlob"], Is.EqualTo(new byte[] { 0xc0, 0xc1, 0xc2 }));
            Assert.That(loadedRow["TheMediumBlob"], Is.TypeOf<byte[]>());

            Assert.That(loadedRow["TheLongBlob"], Is.EqualTo(new byte[] { 0xd0, 0xd1, 0xd2 }));
            Assert.That(loadedRow["TheLongBlob"], Is.TypeOf<byte[]>());
        }

        [Test]
        public void UpdateRow_PropertySelectorDoesNotContainPkColumn_ThrowsArgumentException()
        {
            // Arrange
            this.CreateSuperTable();

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            this.InsertSuperTableRow();

            var update = new
            {
                TheGuid = new Guid("22222222-2222-2222-2222-222222222222"),
            };

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => cruder.UpdateRow("SuperTable", update));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("'rowUpdate' does not contain primary key value."));
            Assert.That(ex.ParamName, Is.EqualTo("rowUpdate"));
        }

        [Test]
        public void UpdateRow_PropertySelectorContainsOnlyPkColumn_ThrowsArgumentException()
        {
            // Arrange
            this.CreateSuperTable();

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            this.InsertSuperTableRow();

            var update = new
            {
                TheInt = 1,
            };

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => cruder.UpdateRow("SuperTable", update));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("'rowUpdate' has no columns to update."));
            Assert.That(ex.ParamName, Is.EqualTo("rowUpdate"));
        }

        [Test]
        public void UpdateRow_IdIsNull_ThrowsArgumentException()
        {
            // Arrange
            this.CreateSuperTable();

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            this.InsertSuperTableRow();

            var update = new
            {
                TheInt = (object)null,
                TheGuid = Guid.NewGuid(),
            };

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => cruder.UpdateRow("SuperTable", update));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("Primary key column value must not be null."));
            Assert.That(ex.ParamName, Is.EqualTo("rowUpdate"));
        }

        [Test]
        public void UpdateRow_NoColumnForSelectedProperty_ThrowsTauDbException()
        {
            // Arrange
            this.CreateSuperTable();
            this.InsertSuperTableRow();

            var update = new
            {
                TheInt = 1,
                NotExisting = 7,
            };

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.UpdateRow("SuperTable", update));

            // Assert
            Assert.That(ex, Has.Message.EqualTo("Column 'NotExisting' does not exist."));
        }

        [Test]
        public void UpdateRow_TableHasNoPrimaryKey_ThrowsArgumentException()
        {
            // Arrange
            this.Connection.ExecuteSingleSql("CREATE TABLE `zeta`.`dummy`(Foo int)"); // no PK

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentException>((() => cruder.UpdateRow("dummy", new { Foo = 1 })));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("Table 'dummy' does not have a primary key."));
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void UpdateRow_TablePrimaryKeyIsMultiColumn_ThrowsArgumentException()
        {
            // Arrange
            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentException>((() => cruder.UpdateRow("Person", new { Key = 3 })));

            // Assert
            Assert.That(ex,
                Has.Message.StartsWith("Failed to retrieve single primary key column name for the table 'person'."));
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        #endregion

        #region DeleteRow

        [Test]
        public void DeleteRow_ValidArguments_DeletesRowAndReturnsTrue()
        {
            // Arrange
            this.CreateMediumTable();

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            const int id = 1;
            this.Connection.ExecuteSingleSql($"INSERT INTO `zeta`.`MediumTable`(`Id`) VALUES ({id})");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var deleted = cruder.DeleteRow("MediumTable", id);

            // Assert
            var deletedRow = TestHelper.LoadRow(this.Connection, "MediumTable", id);

            Assert.That(deleted, Is.True);
            Assert.That(deletedRow, Is.Null);
        }

        [Test]
        public void DeleteRow_IdNotFound_ReturnsFalse()
        {
            // Arrange
            this.CreateMediumTable();

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);
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
            this.Connection.CreateSchema("bad_schema");

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("bad_schema");
            this.Connection.ExecuteSingleSql("CREATE TABLE some_table(id int PRIMARY KEY)");

            IDbCruder cruder = new MySqlCruder(this.Connection);
            this.Connection.DropSchema("bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.DeleteRow("some_table", 17));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void DeleteRow_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<TauDbException>(() => cruder.DeleteRow("bad_table", 17));

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        [Test]
        public void DeleteRow_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.DeleteRow(null, 11));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        [Test]
        public void DeleteRow_IdIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => cruder.DeleteRow("MediumTable", null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("id"));
        }

        [Test]
        public void DeleteRow_TableHasNoPrimaryKey_ThrowsArgumentException()
        {
            // Arrange
            this.Connection.ExecuteSingleSql("CREATE TABLE `zeta`.`dummy`(Foo int)"); // no PK

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

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
            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            IDbCruder cruder = new MySqlCruder(this.Connection);

            // Act

            var ex = Assert.Throws<ArgumentException>((() => cruder.DeleteRow("person", "the_id")));

            // Assert
            Assert.That(
                ex,
                Has.Message.StartsWith("Failed to retrieve single primary key column name for the table 'person'."));
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        #endregion
    }
}
