using MySql.Data.MySqlClient;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;
using TauCode.Extensions;

namespace TauCode.Lab.Db.MySql.Tests.DbTableInspector
{
    // todo: all types (supertable), check returned column types
    // todo: MySqlGuidDbValueConverter

    [TestFixture]
    public class MySqlTableInspectorTests : TestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Connection.CreateSchema("zeta");

            this.Connection.Dispose();
            this.Connection = TestHelper.CreateConnection("zeta");

            var sql = this.GetType().Assembly.GetResourceText("crebase.sql", true);
            this.Connection.ExecuteCommentedScript(sql);
        }

        private void AssertColumn(
            ColumnMold actualColumnMold,
            string expectedColumnName,
            DbTypeMoldInfo expectedType,
            bool expectedIsNullable,
            ColumnIdentityMoldInfo expectedIdentity,
            string expectedDefault)
        {
            Assert.That(actualColumnMold.Name, Is.EqualTo(expectedColumnName));

            Assert.That(actualColumnMold.Type.Name, Is.EqualTo(expectedType.Name));
            Assert.That(actualColumnMold.Type.Size, Is.EqualTo(expectedType.Size));
            Assert.That(actualColumnMold.Type.Precision, Is.EqualTo(expectedType.Precision));
            Assert.That(actualColumnMold.Type.Scale, Is.EqualTo(expectedType.Scale));

            Assert.That(actualColumnMold.IsNullable, Is.EqualTo(expectedIsNullable));

            if (actualColumnMold.Identity == null)
            {
                Assert.That(expectedIdentity, Is.Null);
            }
            else
            {
                Assert.That(expectedIdentity, Is.Not.Null);
                Assert.That(actualColumnMold.Identity.Seed, Is.EqualTo(expectedIdentity.Seed));
                Assert.That(actualColumnMold.Identity.Increment, Is.EqualTo(expectedIdentity.Increment));
            }

            Assert.That(actualColumnMold.Default, Is.EqualTo(expectedDefault));
        }

        #region Constructor

        [Test]
        public void Constructor_ValidArguments_RunsOk()
        {
            // Arrange

            // Act
            IDbTableInspector inspector = new MySqlTableInspectorLab(this.Connection, "tab1");

            // Assert
            Assert.That(inspector.Connection, Is.SameAs(this.Connection));
            Assert.That(inspector.Factory, Is.SameAs(MySqlUtilityFactoryLab.Instance));

            Assert.That(inspector.SchemaName, Is.EqualTo("zeta"));
            Assert.That(inspector.TableName, Is.EqualTo("tab1"));
        }

        [Test]
        public void Constructor_ConnectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new MySqlTableInspectorLab(null, "tab1"));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
        }

        [Test]
        public void Constructor_ConnectionIsNotOpen_ThrowsArgumentException()
        {
            // Arrange
            using var connection = new MySqlConnection(TestHelper.ConnectionString);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => new MySqlTableInspectorLab(connection, "tab1"));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("Connection should be opened."));
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
        }

        [Test]
        public void Constructor_SchemaIsNull_RunsOkAndSchemaIsDbo()
        {
            // Arrange

            // Act
            IDbTableInspector inspector = new MySqlTableInspectorLab(this.Connection, "tab1");

            // Assert
            Assert.That(inspector.Connection, Is.SameAs(this.Connection));
            Assert.That(inspector.Factory, Is.SameAs(MySqlUtilityFactoryLab.Instance));

            Assert.That(inspector.SchemaName, Is.EqualTo("zeta"));
            Assert.That(inspector.TableName, Is.EqualTo("tab1"));
        }

        [Test]
        public void Constructor_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new MySqlTableInspectorLab(this.Connection, null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("tableName"));
        }

        #endregion

        #region GetColumns

        [Test]
        public void GetColumns_ValidInput_ReturnsColumns()
        {
            // Arrange
            var tableNames = new[]
            {
                "Person",
                "PersonData",
                "WorkInfo",
                "TaxInfo",
                "HealthInfo",
                "NumericData",
                "DateData",
            };


            // Act
            var dictionary = tableNames
                .Select(x => new MySqlTableInspectorLab(this.Connection, x))
                .ToDictionary(x => x.TableName, x => x.GetColumns());

            // Assert

            IReadOnlyList<ColumnMold> columns;

            #region Person

            columns = dictionary["Person"];
            Assert.That(columns, Has.Count.EqualTo(8));

            this.AssertColumn(columns[0], "MetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[1], "OrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);
            this.AssertColumn(columns[2], "Id", new DbTypeMoldInfo("bigint"), false, null, null);

            this.AssertColumn(columns[3], "FirstName", new DbTypeMoldInfo("varchar", size: 100), false, null, null);
            Assert.That(columns[3].Type.Properties["character_set_name"], Is.EqualTo("utf8mb4"));
            Assert.That(columns[3].Type.Properties["collation_name"], Is.EqualTo("utf8mb4_0900_ai_ci"));


            this.AssertColumn(columns[4], "LastName", new DbTypeMoldInfo("varchar", size: 100), false, null, null);
            Assert.That(columns[4].Type.Properties["character_set_name"], Is.EqualTo("utf8mb4"));
            Assert.That(columns[4].Type.Properties["collation_name"], Is.EqualTo("utf8mb4_0900_ai_ci"));

            this.AssertColumn(columns[5], "Birthday", new DbTypeMoldInfo("date"), false, null, null);
            this.AssertColumn(columns[6], "Gender", new DbTypeMoldInfo("tinyint"), true, null, null);

            this.AssertColumn(columns[7], "Initials", new DbTypeMoldInfo("char", size: 2), true, null, null);
            Assert.That(columns[7].Type.Properties["character_set_name"], Is.EqualTo("utf8mb4"));
            Assert.That(columns[7].Type.Properties["collation_name"], Is.EqualTo("utf8mb4_0900_ai_ci"));

            #endregion

            #region PersonData

            columns = dictionary["PersonData"];
            Assert.That(columns, Has.Count.EqualTo(8));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("char", 16), false, null, null);
            Assert.That(columns[0].Type.Properties["character_set_name"], Is.EqualTo("ascii"));
            Assert.That(columns[0].Type.Properties["collation_name"], Is.EqualTo("ascii_bin"));

            this.AssertColumn(columns[1], "Height", new DbTypeMoldInfo("int"), true, null, null);
            this.AssertColumn(columns[2], "Photo", new DbTypeMoldInfo("blob"), true, null, null);
            this.AssertColumn(columns[3], "EnglishDescription", new DbTypeMoldInfo("text"), false, null,
                null);
            Assert.That(columns[3].Type.Properties["character_set_name"], Is.EqualTo("ascii"));
            Assert.That(columns[3].Type.Properties["collation_name"], Is.EqualTo("ascii_general_ci"));

            this.AssertColumn(columns[4], "UnicodeDescription", new DbTypeMoldInfo("text"), false, null,
                null);
            Assert.That(columns[4].Type.Properties["character_set_name"], Is.EqualTo("utf8mb4"));
            Assert.That(columns[4].Type.Properties["collation_name"], Is.EqualTo("utf8mb4_0900_ai_ci"));

            this.AssertColumn(columns[5], "PersonMetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[6], "PersonOrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);
            this.AssertColumn(columns[7], "PersonId", new DbTypeMoldInfo("bigint"), false, null, null);

            #endregion

            #region WorkInfo

            columns = dictionary["WorkInfo"];
            Assert.That(columns, Has.Count.EqualTo(11));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("char", 16), false, null, null);
            Assert.That(columns[0].Type.Properties["character_set_name"], Is.EqualTo("ascii"));
            Assert.That(columns[0].Type.Properties["collation_name"], Is.EqualTo("ascii_bin"));

            this.AssertColumn(columns[1], "Position", new DbTypeMoldInfo("varchar", size: 20), false, null, null);
            Assert.That(columns[1].Type.Properties["character_set_name"], Is.EqualTo("ascii"));
            Assert.That(columns[1].Type.Properties["collation_name"], Is.EqualTo("ascii_general_ci"));

            this.AssertColumn(columns[2], "HireDate", new DbTypeMoldInfo("datetime"), false, null, null);

            this.AssertColumn(columns[3], "Code", new DbTypeMoldInfo("char", size: 3), true, null, null);
            Assert.That(columns[3].Type.Properties["character_set_name"], Is.EqualTo("ascii"));
            Assert.That(columns[3].Type.Properties["collation_name"], Is.EqualTo("ascii_general_ci"));

            this.AssertColumn(columns[4], "PersonMetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[5], "DigitalSignature", new DbTypeMoldInfo("binary", size: 16), false, null,
                null);
            this.AssertColumn(columns[6], "PersonId", new DbTypeMoldInfo("bigint"), false, null, null);
            this.AssertColumn(columns[7], "PersonOrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);

            this.AssertColumn(columns[8], "Hash", new DbTypeMoldInfo("char", 16), false, null, null);
            Assert.That(columns[8].Type.Properties["character_set_name"], Is.EqualTo("ascii"));
            Assert.That(columns[8].Type.Properties["collation_name"], Is.EqualTo("ascii_bin"));

            this.AssertColumn(columns[9], "Salary", new DbTypeMoldInfo("decimal", null, 13, 4), true, null, null);
            this.AssertColumn(columns[10], "VaryingSignature", new DbTypeMoldInfo("varbinary", size: 100), true, null,
                null);

            #endregion

            #region TaxInfo

            columns = dictionary["TaxInfo"];
            Assert.That(columns, Has.Count.EqualTo(10));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("char", 16), false, null, null);
            Assert.That(columns[0].Type.Properties["character_set_name"], Is.EqualTo("ascii"));
            Assert.That(columns[0].Type.Properties["collation_name"], Is.EqualTo("ascii_bin"));

            this.AssertColumn(columns[1], "PersonId", new DbTypeMoldInfo("bigint"), false, null, null);
            this.AssertColumn(columns[2], "Tax", new DbTypeMoldInfo("decimal", null, 13, 4), false, null, null);
            this.AssertColumn(columns[3], "Ratio", new DbTypeMoldInfo("double"), true, null, null);
            this.AssertColumn(columns[4], "PersonMetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[5], "SmallRatio", new DbTypeMoldInfo("float"), false, null, null);
            this.AssertColumn(columns[6], "RecordDate", new DbTypeMoldInfo("datetime"), true, null, null);
            this.AssertColumn(columns[7], "CreatedAt", new DbTypeMoldInfo("datetime"), false, null, null);
            this.AssertColumn(columns[8], "PersonOrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);
            this.AssertColumn(columns[9], "DueDate", new DbTypeMoldInfo("datetime"), true, null, null);

            #endregion

            #region HealthInfo

            columns = dictionary["HealthInfo"];
            Assert.That(columns, Has.Count.EqualTo(9));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("char", 16), false, null, null);
            // todo: binary

            this.AssertColumn(columns[1], "PersonId", new DbTypeMoldInfo("bigint"), false, null, null);
            this.AssertColumn(
                columns[2],
                "Weight",
                new DbTypeMoldInfo("decimal", precision: 8, scale: 2),
                false,
                null,
                null);
            this.AssertColumn(columns[3], "PersonMetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[4], "IQ", new DbTypeMoldInfo("decimal", precision: 8, scale: 2), true, null, null);
            this.AssertColumn(columns[5], "Temper", new DbTypeMoldInfo("smallint"), true, null, null);
            this.AssertColumn(columns[6], "PersonOrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);
            this.AssertColumn(columns[7], "MetricB", new DbTypeMoldInfo("int"), true, null, null);
            this.AssertColumn(columns[8], "MetricA", new DbTypeMoldInfo("int"), true, null, null);

            #endregion

            #region NumericData

            columns = dictionary["NumericData"];
            Assert.That(columns, Has.Count.EqualTo(10));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("int"), false, new ColumnIdentityMoldInfo("1", "1"), null);
            this.AssertColumn(columns[1], "BooleanData", new DbTypeMoldInfo("tinyint"), true, null, null);
            this.AssertColumn(columns[2], "ByteData", new DbTypeMoldInfo("tinyint"), true, null, null);
            this.AssertColumn(columns[3], "Int16", new DbTypeMoldInfo("smallint"), true, null, null);
            this.AssertColumn(columns[4], "Int32", new DbTypeMoldInfo("int"), true, null, null);
            this.AssertColumn(columns[5], "Int64", new DbTypeMoldInfo("bigint"), true, null, null);
            this.AssertColumn(columns[6], "NetDouble", new DbTypeMoldInfo("double"), true, null, null);
            this.AssertColumn(columns[7], "NetSingle", new DbTypeMoldInfo("float"), true, null, null);
            this.AssertColumn(columns[8], "NumericData", new DbTypeMoldInfo("decimal", precision: 10, scale: 6), true, null, null);
            this.AssertColumn(columns[9], "DecimalData", new DbTypeMoldInfo("decimal", precision: 11, scale: 5), true, null, null);

            #endregion

            #region DateData

            columns = dictionary["DateData"];
            Assert.That(columns, Has.Count.EqualTo(2));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("char", 16), false, null, null);

            this.AssertColumn(columns[1], "Moment", new DbTypeMoldInfo("datetime"), true, null, null);

            #endregion
        }

        [Test]
        public void GetColumns_AllTypes_ReturnsColumns()
        {
            // Arrange
            this.Connection.Purge();
            this.Connection.CreateSchema("zeta");

            var sql = this.GetType().Assembly.GetResourceText("SuperTable.sql", true);
            this.Connection.ExecuteSingleSql(sql);
            using var connection = TestHelper.CreateConnection("zeta");
            var tableInspector = new MySqlTableInspectorLab(connection, "supertable");

            // Act
            var columns = tableInspector.GetColumns();


            // Assert

            Assert.That(columns, Has.Count.EqualTo(35));

            // TheInt
            this.AssertColumn(columns[0], "TheInt", new DbTypeMoldInfo("int"), false, new ColumnIdentityMoldInfo("1", "1"), null);
            Assert.That(columns[0].Properties, Is.Empty);

            // TheIntUnsigned
            this.AssertColumn(columns[1], "TheIntUnsigned", new DbTypeMoldInfo("int"), true, null, null);
            Assert.That(columns[1].Type.Properties["unsigned"], Is.EqualTo("true"));

            // TheBit
            this.AssertColumn(columns[2], "TheBit", new DbTypeMoldInfo("bit", precision: 1), true, null, null);
            Assert.That(columns[2].Type.Properties, Is.Empty);

            // TheBit9
            this.AssertColumn(columns[3], "TheBit9", new DbTypeMoldInfo("bit", precision: 9), true, null, null);
            Assert.That(columns[3].Type.Properties, Is.Empty);

            // TheTinyInt
            this.AssertColumn(columns[4], "TheTinyInt", new DbTypeMoldInfo("tinyint"), true, null, null);
            Assert.That(columns[4].Type.Properties, Is.Empty);

            // TheTinyIntUnsigned
            this.AssertColumn(columns[5], "TheTinyIntUnsigned", new DbTypeMoldInfo("tinyint"), true, null, null);
            Assert.That(columns[5].Type.Properties["unsigned"], Is.EqualTo("true"));

            // TheBool
            this.AssertColumn(columns[6], "TheBool", new DbTypeMoldInfo("tinyint"), true, null, null);
            Assert.That(columns[6].Type.Properties, Is.Empty);

            // TheBoolean
            this.AssertColumn(columns[7], "TheBoolean", new DbTypeMoldInfo("tinyint"), true, null, null);
            Assert.That(columns[7].Type.Properties, Is.Empty);

            // TheSmallInt
            this.AssertColumn(columns[8], "TheSmallInt", new DbTypeMoldInfo("smallint"), true, null, null);
            Assert.That(columns[8].Type.Properties, Is.Empty);

            // TheSmallIntUnsigned
            this.AssertColumn(columns[9], "TheSmallIntUnsigned", new DbTypeMoldInfo("smallint"), true, null, null);
            Assert.That(columns[9].Type.Properties["unsigned"], Is.EqualTo("true"));

            // TheMediumInt
            this.AssertColumn(columns[10], "TheMediumInt", new DbTypeMoldInfo("mediumint"), true, null, null);
            Assert.That(columns[10].Type.Properties, Is.Empty);

            // TheMediumIntUnsigned
            this.AssertColumn(columns[11], "TheMediumIntUnsigned", new DbTypeMoldInfo("mediumint"), true, null, null);
            Assert.That(columns[11].Type.Properties["unsigned"], Is.EqualTo("true"));

            // TheBigInt
            this.AssertColumn(columns[12], "TheBigInt", new DbTypeMoldInfo("bigint"), true, null, null);
            Assert.That(columns[12].Type.Properties, Is.Empty);

            // TheBigIntUnsigned
            this.AssertColumn(columns[13], "TheBigIntUnsigned", new DbTypeMoldInfo("bigint"), true, null, null);
            Assert.That(columns[13].Type.Properties["unsigned"], Is.EqualTo("true"));

            // TheDecimal
            this.AssertColumn(columns[14], "TheDecimal", new DbTypeMoldInfo("decimal", null, 8, 2), true, null, null);
            Assert.That(columns[14].Type.Properties, Is.Empty);

            // TheNumeric
            this.AssertColumn(columns[15], "TheNumeric", new DbTypeMoldInfo("decimal", null, 10, 3), true, null, null);
            Assert.That(columns[15].Type.Properties, Is.Empty);

            // TheFloat
            this.AssertColumn(columns[16], "TheFloat", new DbTypeMoldInfo("float"), true, null, null);
            Assert.That(columns[16].Type.Properties, Is.Empty);

            // TheDouble
            this.AssertColumn(columns[17], "TheDouble", new DbTypeMoldInfo("double"), true, null, null);
            Assert.That(columns[17].Type.Properties, Is.Empty);

            // TheDate
            this.AssertColumn(columns[18], "TheDate", new DbTypeMoldInfo("date"), true, null, null);
            Assert.That(columns[18].Type.Properties, Is.Empty);

            // TheDateTime
            this.AssertColumn(columns[19], "TheDateTime", new DbTypeMoldInfo("datetime"), true, null, null);
            Assert.That(columns[19].Type.Properties, Is.Empty);

            // TheTimeStamp
            this.AssertColumn(columns[20], "TheTimeStamp", new DbTypeMoldInfo("timestamp"), true, null, null);
            Assert.That(columns[20].Type.Properties, Is.Empty);

            // TheTime
            this.AssertColumn(columns[21], "TheTime", new DbTypeMoldInfo("time"), true, null, null);
            Assert.That(columns[21].Type.Properties, Is.Empty);

            // TheYear
            this.AssertColumn(columns[22], "TheYear", new DbTypeMoldInfo("year"), true, null, null);
            Assert.That(columns[22].Type.Properties, Is.Empty);

            // TheChar
            this.AssertColumn(columns[23], "TheChar", new DbTypeMoldInfo("char", 100), true, null, null);
            Assert.That(columns[23].Type.Properties["character_set_name"], Is.EqualTo("ascii"));
            Assert.That(columns[23].Type.Properties["collation_name"], Is.EqualTo("ascii_general_ci"));

            // TheVarChar
            this.AssertColumn(columns[24], "TheVarChar", new DbTypeMoldInfo("varchar", 100), true, null, null);
            Assert.That(columns[24].Type.Properties["character_set_name"], Is.EqualTo("utf8mb4"));
            Assert.That(columns[24].Type.Properties["collation_name"], Is.EqualTo("utf8mb4_0900_ai_ci"));

            // TheBinary
            this.AssertColumn(columns[25], "TheBinary", new DbTypeMoldInfo("binary", 10), true, null, null);
            Assert.That(columns[25].Type.Properties, Is.Empty);

            // TheVarBinary
            this.AssertColumn(columns[26], "TheVarBinary", new DbTypeMoldInfo("varbinary", 20), true, null, null);
            Assert.That(columns[26].Type.Properties, Is.Empty);

            // TheTinyText
            this.AssertColumn(columns[27], "TheTinyText", new DbTypeMoldInfo("tinytext"), true, null, null);
            Assert.That(columns[27].Type.Properties["character_set_name"], Is.EqualTo("ascii"));
            Assert.That(columns[27].Type.Properties["collation_name"], Is.EqualTo("ascii_general_ci"));

            // TheText
            this.AssertColumn(columns[28], "TheText", new DbTypeMoldInfo("text"), true, null, null);
            Assert.That(columns[28].Type.Properties["character_set_name"], Is.EqualTo("utf8mb4"));
            Assert.That(columns[28].Type.Properties["collation_name"], Is.EqualTo("utf8mb4_0900_ai_ci"));

            // TheMediumText
            this.AssertColumn(columns[29], "TheMediumText", new DbTypeMoldInfo("mediumtext"), true, null, null);
            Assert.That(columns[29].Type.Properties["character_set_name"], Is.EqualTo("ascii"));
            Assert.That(columns[29].Type.Properties["collation_name"], Is.EqualTo("ascii_general_ci"));

            // TheLongText
            this.AssertColumn(columns[30], "TheLongText", new DbTypeMoldInfo("longtext"), true, null, null);
            Assert.That(columns[30].Type.Properties["character_set_name"], Is.EqualTo("utf8mb4"));
            Assert.That(columns[30].Type.Properties["collation_name"], Is.EqualTo("utf8mb4_0900_ai_ci"));

            // TheTinyBlob
            this.AssertColumn(columns[31], "TheTinyBlob", new DbTypeMoldInfo("tinyblob"), true, null, null);
            Assert.That(columns[31].Type.Properties, Is.Empty);

            // TheBlob
            this.AssertColumn(columns[32], "TheBlob", new DbTypeMoldInfo("blob"), true, null, null);
            Assert.That(columns[32].Type.Properties, Is.Empty);

            // TheMediumBlob
            this.AssertColumn(columns[33], "TheMediumBlob", new DbTypeMoldInfo("mediumblob"), true, null, null);
            Assert.That(columns[33].Type.Properties, Is.Empty);

            // TheLongBlob
            this.AssertColumn(columns[34], "TheLongBlob", new DbTypeMoldInfo("longblob"), true, null, null);
            Assert.That(columns[34].Type.Properties, Is.Empty);
        }

        [Test]
        public void GetColumns_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            this.Connection.CreateSchema("bad_schema");
            using var connection = TestHelper.CreateConnection("bad_schema");
            this.Connection.DropSchema("bad_schema");

            IDbTableInspector inspector = new MySqlTableInspectorLab(connection, "tab1");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetColumns());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetColumns_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new MySqlTableInspectorLab(this.Connection, "bad_table");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetColumns());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        #endregion

        #region GetPrimaryKey

        [Test]
        public void GetPrimaryKey_ValidInput_ReturnsPrimaryKey()
        {
            // Arrange
            var tableNames = this.Connection.GetTableNames("zeta", null);

            // Act
            var dictionary = tableNames
                .Select(x => new MySqlTableInspectorLab(this.Connection, x))
                .ToDictionary(x => x.TableName, x => x.GetPrimaryKey());

            // Assert

            PrimaryKeyMold pk;

            // Person
            pk = dictionary["person"];
            Assert.That(pk.Name, Is.EqualTo("PRIMARY"));
            CollectionAssert.AreEqual(
                new[]
                {
                    "Id",
                    "MetaKey",
                    "OrdNumber",
                },
                pk.Columns);

            // PersonData
            pk = dictionary["persondata"];
            Assert.That(pk.Name, Is.EqualTo("PRIMARY"));
            CollectionAssert.AreEqual(
                new[]
                {
                    "Id",
                },
                pk.Columns);

            // NumericData
            pk = dictionary["numericdata"];
            Assert.That(pk.Name, Is.EqualTo("PRIMARY"));
            CollectionAssert.AreEqual(
                new[]
                {
                    "Id",
                },
                pk.Columns);

        }

        [Test]
        public void GetPrimaryKey_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            this.Connection.CreateSchema("bad_schema");
            using var connection = TestHelper.CreateConnection("bad_schema");
            this.Connection.DropSchema("bad_schema");
            IDbTableInspector inspector = new MySqlTableInspectorLab(connection, "tab1");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetPrimaryKey());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetPrimaryKey_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new MySqlTableInspectorLab(this.Connection, "bad_table");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetPrimaryKey());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        #endregion

        #region GetForeignKeys

        [Test]
        public void GetForeignKeys_ValidInput_ReturnsForeignKeys()
        {
            // Arrange
            IDbTableInspector inspector = new MySqlTableInspectorLab(this.Connection, "PersonData");

            // Act
            var foreignKeys = inspector.GetForeignKeys();

            // Assert
            Assert.That(foreignKeys, Has.Count.EqualTo(1));
            var fk = foreignKeys.Single();

            Assert.That(fk.Name, Is.EqualTo("FK_personData_Person"));
            CollectionAssert.AreEqual(
                new string[] { "PersonId", "PersonMetaKey", "PersonOrdNumber" },
                fk.ColumnNames);
            Assert.That(fk.ReferencedTableName, Is.EqualTo("person"));
            CollectionAssert.AreEqual(
                new string[] { "Id", "MetaKey", "OrdNumber" },
                fk.ReferencedColumnNames);
        }

        [Test]
        public void GetForeignKeys_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            this.Connection.CreateSchema("bad_schema");
            using var connection = TestHelper.CreateConnection("bad_schema");
            this.Connection.DropSchema("bad_schema");
            IDbTableInspector inspector = new MySqlTableInspectorLab(connection, "tab1");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetForeignKeys());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetForeignKeys_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new MySqlTableInspectorLab(this.Connection, "bad_table");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetForeignKeys());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        #endregion

        #region GetIndexes

        [Test]
        public void GetIndexes_ValidInput_ReturnsIndexes()
        {
            // Arrange
            IDbTableInspector inspector1 = new MySqlTableInspectorLab(this.Connection, "Person");
            IDbTableInspector inspector2 = new MySqlTableInspectorLab(this.Connection, "WorkInfo");
            IDbTableInspector inspector3 = new MySqlTableInspectorLab(this.Connection, "HealthInfo");

            // Act
            var indexes1 = inspector1.GetIndexes();
            var indexes2 = inspector2.GetIndexes();
            var indexes3 = inspector3.GetIndexes();

            // Assert

            // Person
            var index = indexes1.Single();
            Assert.That(index.Name, Is.EqualTo("PRIMARY"));
            Assert.That(index.TableName, Is.EqualTo("person"));
            Assert.That(index.IsUnique, Is.True);
            Assert.That(index.Columns, Has.Count.EqualTo(3));

            var column = index.Columns[0];
            Assert.That(column.Name, Is.EqualTo("Id"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            column = index.Columns[1];
            Assert.That(column.Name, Is.EqualTo("MetaKey"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            column = index.Columns[2];
            Assert.That(column.Name, Is.EqualTo("OrdNumber"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            // WorkInfo
            Assert.That(indexes2, Has.Count.EqualTo(3));

            // index: FK_workInfo_Person
            index = indexes2[0];
            Assert.That(index.Name, Is.EqualTo("FK_workInfo_Person"));
            Assert.That(index.TableName, Is.EqualTo("workinfo"));
            Assert.That(index.IsUnique, Is.False);
            Assert.That(index.Columns, Has.Count.EqualTo(3));

            column = index.Columns[0];
            Assert.That(column.Name, Is.EqualTo("PersonId"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            column = index.Columns[1];
            Assert.That(column.Name, Is.EqualTo("PersonMetaKey"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            column = index.Columns[2];
            Assert.That(column.Name, Is.EqualTo("PersonOrdNumber"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            // index: PRIMARY
            index = indexes2[1];
            Assert.That(index.Name, Is.EqualTo("PRIMARY"));
            Assert.That(index.TableName, Is.EqualTo("workinfo"));
            Assert.That(index.IsUnique, Is.True);
            Assert.That(index.Columns, Has.Count.EqualTo(1));

            column = index.Columns.Single();
            Assert.That(column.Name, Is.EqualTo("Id"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            // index: UX_workInfo_Hash
            index = indexes2[2];
            Assert.That(index.Name, Is.EqualTo("UX_workInfo_Hash"));
            Assert.That(index.TableName, Is.EqualTo("workinfo"));
            Assert.That(index.IsUnique, Is.True);
            Assert.That(index.Columns, Has.Count.EqualTo(1));

            column = index.Columns.Single();
            Assert.That(column.Name, Is.EqualTo("Hash"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            // HealthInfo
            Assert.That(indexes3, Has.Count.EqualTo(3));

            // index: FK_healthInfo_Person
            index = indexes3[0];
            Assert.That(index.Name, Is.EqualTo("FK_healthInfo_Person"));
            Assert.That(index.TableName, Is.EqualTo("healthinfo"));
            Assert.That(index.IsUnique, Is.False);
            Assert.That(index.Columns, Has.Count.EqualTo(3));

            column = index.Columns[0];
            Assert.That(column.Name, Is.EqualTo("PersonId"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            column = index.Columns[1];
            Assert.That(column.Name, Is.EqualTo("PersonMetaKey"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            column = index.Columns[2];
            Assert.That(column.Name, Is.EqualTo("PersonOrdNumber"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            // index: IX_healthInfo_metricAmetricB
            index = indexes3[1];
            Assert.That(index.Name, Is.EqualTo("IX_healthInfo_metricAmetricB"));
            Assert.That(index.TableName, Is.EqualTo("healthinfo"));
            Assert.That(index.IsUnique, Is.False);
            Assert.That(index.Columns, Has.Count.EqualTo(2));

            column = index.Columns[0];
            Assert.That(column.Name, Is.EqualTo("MetricA"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));

            column = index.Columns[1];
            Assert.That(column.Name, Is.EqualTo("MetricB"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Descending));

            // index: PK_healthInfo
            index = indexes3[2];
            Assert.That(index.Name, Is.EqualTo("PRIMARY"));
            Assert.That(index.TableName, Is.EqualTo("healthinfo"));
            Assert.That(index.IsUnique, Is.True);
            Assert.That(index.Columns, Has.Count.EqualTo(1));

            column = index.Columns.Single();
            Assert.That(column.Name, Is.EqualTo("Id"));
            Assert.That(column.SortDirection, Is.EqualTo(SortDirection.Ascending));
        }

        [Test]
        public void GetIndexes_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            this.Connection.CreateSchema("bad_schema");
            using var connection = TestHelper.CreateConnection("bad_schema");
            IDbTableInspector inspector = new MySqlTableInspectorLab(connection, "tab1");
            this.Connection.DropSchema("bad_schema");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetForeignKeys());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetIndexes_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new MySqlTableInspectorLab(this.Connection, "bad_table");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetForeignKeys());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        #endregion

        #region GetTable

        [Test]
        public void GetTable_ValidInput_ReturnsTable()
        {
            // Arrange
            IDbTableInspector inspector = new MySqlTableInspectorLab(this.Connection, "HealthInfo");

            // Act
            var table = inspector.GetTable();

            // Assert

            Assert.That(table.Name, Is.EqualTo("HealthInfo"));

            #region primary keys

            var primaryKey = table.PrimaryKey;

            Assert.That(primaryKey.Name, Is.EqualTo("PRIMARY"));

            Assert.That(primaryKey.Columns, Has.Count.EqualTo(1));

            var column = primaryKey.Columns.Single();
            Assert.That(column, Is.EqualTo("Id"));

            #endregion

            #region columns

            var columns = table.Columns;
            Assert.That(columns, Has.Count.EqualTo(9));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("char", 16), false, null, null);
            // todo: binary

            this.AssertColumn(columns[1], "PersonId", new DbTypeMoldInfo("bigint"), false, null, null);
            this.AssertColumn(
                columns[2],
                "Weight",
                new DbTypeMoldInfo("decimal", precision: 8, scale: 2),
                false,
                null,
                null);
            this.AssertColumn(columns[3], "PersonMetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[4], "IQ", new DbTypeMoldInfo("decimal", precision: 8, scale: 2), true, null, null);
            this.AssertColumn(columns[5], "Temper", new DbTypeMoldInfo("smallint"), true, null, null);
            this.AssertColumn(columns[6], "PersonOrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);
            this.AssertColumn(columns[7], "MetricB", new DbTypeMoldInfo("int"), true, null, null);
            this.AssertColumn(columns[8], "MetricA", new DbTypeMoldInfo("int"), true, null, null);

            #endregion

            #region foreign keys

            var foreignKeys = table.ForeignKeys;

            Assert.That(foreignKeys, Has.Count.EqualTo(1));
            var fk = foreignKeys.Single();

            Assert.That(fk.Name, Is.EqualTo("FK_healthInfo_Person"));
            CollectionAssert.AreEqual(
                new string[]
                {
                    "PersonId",
                    "PersonMetaKey",
                    "PersonOrdNumber",
                },
                fk.ColumnNames);

            Assert.That(fk.ReferencedTableName, Is.EqualTo("person"));
            CollectionAssert.AreEqual(
                new string[]
                {
                    "Id",
                    "MetaKey",
                    "OrdNumber",
                },
                fk.ReferencedColumnNames);

            #endregion

            #region indexes

            var indexes = table.Indexes;

            Assert.That(indexes, Has.Count.EqualTo(3));

            // index: FK_healthInfo_Person
            var index = indexes[0];
            Assert.That(index.Name, Is.EqualTo("FK_healthInfo_Person"));
            Assert.That(index.TableName, Is.EqualTo("healthinfo"));
            Assert.That(index.IsUnique, Is.False);
            Assert.That(index.Columns, Has.Count.EqualTo(3));

            var indexColumn = index.Columns[0];
            Assert.That(indexColumn.Name, Is.EqualTo("PersonId"));
            Assert.That(indexColumn.SortDirection, Is.EqualTo(SortDirection.Ascending));

            indexColumn = index.Columns[1];
            Assert.That(indexColumn.Name, Is.EqualTo("PersonMetaKey"));
            Assert.That(indexColumn.SortDirection, Is.EqualTo(SortDirection.Ascending));

            indexColumn = index.Columns[2];
            Assert.That(indexColumn.Name, Is.EqualTo("PersonOrdNumber"));
            Assert.That(indexColumn.SortDirection, Is.EqualTo(SortDirection.Ascending));

            // index: IX_healthInfo_metricAmetricB
            index = indexes[1];
            Assert.That(index.Name, Is.EqualTo("IX_healthInfo_metricAmetricB"));
            Assert.That(index.TableName, Is.EqualTo("healthinfo"));
            Assert.That(index.IsUnique, Is.False);
            Assert.That(index.Columns, Has.Count.EqualTo(2));

            indexColumn = index.Columns[0];
            Assert.That(indexColumn.Name, Is.EqualTo("MetricA"));
            Assert.That(indexColumn.SortDirection, Is.EqualTo(SortDirection.Ascending));

            indexColumn = index.Columns[1];
            Assert.That(indexColumn.Name, Is.EqualTo("MetricB"));
            Assert.That(indexColumn.SortDirection, Is.EqualTo(SortDirection.Descending));

            // index: PK_healthInfo
            index = indexes[2];
            Assert.That(index.Name, Is.EqualTo("PRIMARY"));
            Assert.That(index.TableName, Is.EqualTo("healthinfo"));
            Assert.That(index.IsUnique, Is.True);
            Assert.That(index.Columns, Has.Count.EqualTo(1));

            indexColumn = index.Columns.Single();
            Assert.That(indexColumn.Name, Is.EqualTo("Id"));
            Assert.That(indexColumn.SortDirection, Is.EqualTo(SortDirection.Ascending));

            #endregion
        }

        [Test]
        public void GetTable_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            this.Connection.CreateSchema("bad_schema");
            using var connection = TestHelper.CreateConnection("bad_schema");
            this.Connection.DropSchema("bad_schema");
            IDbTableInspector inspector = new MySqlTableInspectorLab(connection, "tab1");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetTable());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }

        [Test]
        public void GetTable_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new MySqlTableInspectorLab(this.Connection, "bad_table");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetTable());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        #endregion
    }
}
