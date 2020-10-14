﻿using Microsoft.Data.SqlClient;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;
using TauCode.Extensions;

namespace TauCode.Lab.Db.SqlClient.Tests.DbTableInspector
{
    [TestFixture]
    public class SqlTableInspectorTests : TestBase
    {
        [SetUp]
        public void SetUp()
        {
            this.Connection.CreateSchema("zeta");

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
            IDbTableInspector inspector = new SqlTableInspectorLab(this.Connection, "dbo", "tab1");

            // Assert
            Assert.That(inspector.Connection, Is.SameAs(this.Connection));
            Assert.That(inspector.Factory, Is.SameAs(SqlUtilityFactoryLab.Instance));

            Assert.That(inspector.SchemaName, Is.EqualTo("dbo"));
            Assert.That(inspector.TableName, Is.EqualTo("tab1"));
        }

        [Test]
        public void Constructor_ConnectionIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new SqlTableInspectorLab(null, "dbo", "tab1"));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
        }

        [Test]
        public void Constructor_ConnectionIsNotOpen_ThrowsArgumentException()
        {
            // Arrange
            using var connection = new SqlConnection(TestHelper.ConnectionString);

            // Act
            var ex = Assert.Throws<ArgumentException>(() => new SqlTableInspectorLab(connection, "dbo", "tab1"));

            // Assert
            Assert.That(ex, Has.Message.StartsWith("Connection should be opened."));
            Assert.That(ex.ParamName, Is.EqualTo("connection"));
        }

        [Test]
        public void Constructor_SchemaIsNull_RunsOkAndSchemaIsDbo()
        {
            // Arrange

            // Act
            IDbTableInspector inspector = new SqlTableInspectorLab(this.Connection, null, "tab1");

            // Assert
            Assert.That(inspector.Connection, Is.SameAs(this.Connection));
            Assert.That(inspector.Factory, Is.SameAs(SqlUtilityFactoryLab.Instance));

            Assert.That(inspector.SchemaName, Is.EqualTo("dbo"));
            Assert.That(inspector.TableName, Is.EqualTo("tab1"));
        }

        [Test]
        public void Constructor_TableNameIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => new SqlTableInspectorLab(this.Connection, "dbo", null));

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
                .Select(x => new SqlTableInspectorLab(this.Connection, "zeta", x))
                .ToDictionary(x => x.TableName, x => x.GetColumns());

            // Assert

            IReadOnlyList<ColumnMold> columns;

            #region Person

            columns = dictionary["Person"];
            Assert.That(columns, Has.Count.EqualTo(8));

            this.AssertColumn(columns[0], "MetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[1], "OrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);
            this.AssertColumn(columns[2], "Id", new DbTypeMoldInfo("bigint"), false, null, null);
            this.AssertColumn(columns[3], "FirstName", new DbTypeMoldInfo("nvarchar", size: 100), false, null, null);
            this.AssertColumn(columns[4], "LastName", new DbTypeMoldInfo("nvarchar", size: 100), false, null, null);
            this.AssertColumn(columns[5], "Birthday", new DbTypeMoldInfo("date"), false, null, null);
            this.AssertColumn(columns[6], "Gender", new DbTypeMoldInfo("bit"), true, null, null);
            this.AssertColumn(columns[7], "Initials", new DbTypeMoldInfo("nchar", size: 2), true, null, null);

            #endregion

            #region PersonData

            columns = dictionary["PersonData"];
            Assert.That(columns, Has.Count.EqualTo(8));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("uniqueidentifier"), false, null, null);
            this.AssertColumn(columns[1], "Height", new DbTypeMoldInfo("int"), true, null, null);
            this.AssertColumn(columns[2], "Photo", new DbTypeMoldInfo("varbinary", size: -1), true, null, null);
            this.AssertColumn(columns[3], "EnglishDescription", new DbTypeMoldInfo("varchar", size: -1), false, null,
                null);
            this.AssertColumn(columns[4], "UnicodeDescription", new DbTypeMoldInfo("nvarchar", size: -1), false, null,
                null);
            this.AssertColumn(columns[5], "PersonMetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[6], "PersonOrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);
            this.AssertColumn(columns[7], "PersonId", new DbTypeMoldInfo("bigint"), false, null, null);

            #endregion

            #region WorkInfo

            columns = dictionary["WorkInfo"];
            Assert.That(columns, Has.Count.EqualTo(11));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("uniqueidentifier"), false, null, null);
            this.AssertColumn(columns[1], "Position", new DbTypeMoldInfo("varchar", size: 20), false, null, null);
            this.AssertColumn(columns[2], "HireDate", new DbTypeMoldInfo("smalldatetime"), false, null, null);
            this.AssertColumn(columns[3], "Code", new DbTypeMoldInfo("char", size: 3), true, null, null);
            this.AssertColumn(columns[4], "PersonMetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[5], "DigitalSignature", new DbTypeMoldInfo("binary", size: 16), false, null,
                null);
            this.AssertColumn(columns[6], "PersonId", new DbTypeMoldInfo("bigint"), false, null, null);
            this.AssertColumn(columns[7], "PersonOrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);
            this.AssertColumn(columns[8], "Hash", new DbTypeMoldInfo("uniqueidentifier"), false, null, null);
            this.AssertColumn(columns[9], "Salary", new DbTypeMoldInfo("smallmoney"), true, null, null);
            this.AssertColumn(columns[10], "VaryingSignature", new DbTypeMoldInfo("varbinary", size: 100), true, null,
                null);

            #endregion

            #region TaxInfo

            columns = dictionary["TaxInfo"];
            Assert.That(columns, Has.Count.EqualTo(10));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("uniqueidentifier"), false, null, null);
            this.AssertColumn(columns[1], "PersonId", new DbTypeMoldInfo("bigint"), false, null, null);
            this.AssertColumn(columns[2], "Tax", new DbTypeMoldInfo("money"), false, null, null);
            this.AssertColumn(columns[3], "Ratio", new DbTypeMoldInfo("float"), true, null, null);
            this.AssertColumn(columns[4], "PersonMetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[5], "SmallRatio", new DbTypeMoldInfo("real"), false, null, null);
            this.AssertColumn(columns[6], "RecordDate", new DbTypeMoldInfo("datetime"), true, null, null);
            this.AssertColumn(columns[7], "CreatedAt", new DbTypeMoldInfo("datetimeoffset"), false, null, null);
            this.AssertColumn(columns[8], "PersonOrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);
            this.AssertColumn(columns[9], "DuteDate", new DbTypeMoldInfo("datetime2"), true, null, null);

            #endregion

            #region HealthInfo

            columns = dictionary["HealthInfo"];
            Assert.That(columns, Has.Count.EqualTo(9));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("uniqueidentifier"), false, null, null);
            this.AssertColumn(columns[1], "PersonId", new DbTypeMoldInfo("bigint"), false, null, null);
            this.AssertColumn(
                columns[2],
                "Weight",
                new DbTypeMoldInfo("decimal", precision: SqlToolsLab.DefaultDecimalPrecision, scale: 0),
                false,
                null,
                null);
            this.AssertColumn(columns[3], "PersonMetaKey", new DbTypeMoldInfo("smallint"), false, null, null);
            this.AssertColumn(columns[4], "IQ", new DbTypeMoldInfo("numeric", precision: 8, scale: 2), true, null, null);
            this.AssertColumn(columns[5], "Temper", new DbTypeMoldInfo("smallint"), true, null, null);
            this.AssertColumn(columns[6], "PersonOrdNumber", new DbTypeMoldInfo("tinyint"), false, null, null);
            this.AssertColumn(columns[7], "MetricB", new DbTypeMoldInfo("int"), true, null, null);
            this.AssertColumn(columns[8], "MetricA", new DbTypeMoldInfo("int"), true, null, null);

            #endregion

            #region NumericData

            columns = dictionary["NumericData"];
            Assert.That(columns, Has.Count.EqualTo(10));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("int"), false, new ColumnIdentityMoldInfo("15", "99"), null);
            this.AssertColumn(columns[1], "BooleanData", new DbTypeMoldInfo("bit"), true, null, null);
            this.AssertColumn(columns[2], "ByteData", new DbTypeMoldInfo("tinyint"), true, null, null);
            this.AssertColumn(columns[3], "Int16", new DbTypeMoldInfo("smallint"), true, null, null);
            this.AssertColumn(columns[4], "Int32", new DbTypeMoldInfo("int"), true, null, null);
            this.AssertColumn(columns[5], "Int64", new DbTypeMoldInfo("bigint"), true, null, null);
            this.AssertColumn(columns[6], "NetDouble", new DbTypeMoldInfo("float"), true, null, null);
            this.AssertColumn(columns[7], "NetSingle", new DbTypeMoldInfo("real"), true, null, null);
            this.AssertColumn(columns[8], "NumericData", new DbTypeMoldInfo("numeric", precision: 10, scale: 6), true, null, null);
            this.AssertColumn(columns[9], "DecimalData", new DbTypeMoldInfo("decimal", precision: 11, scale: 5), true, null, null);

            #endregion

            #region DateData

            columns = dictionary["DateData"];
            Assert.That(columns, Has.Count.EqualTo(2));

            this.AssertColumn(columns[0], "Id", new DbTypeMoldInfo("uniqueidentifier"), false, null, null);
            this.AssertColumn(columns[1], "Moment", new DbTypeMoldInfo("datetimeoffset"), true, null, null);

            #endregion
        }

        [Test]
        public void GetColumns_SchemaDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new SqlTableInspectorLab(this.Connection, "bad_schema", "tab1");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetColumns());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Schema 'bad_schema' does not exist."));
        }
        
        [Test]
        public void GetColumns_TableDoesNotExist_ThrowsTauDbException()
        {
            // Arrange
            IDbTableInspector inspector = new SqlTableInspectorLab(this.Connection, "zeta", "bad_table");

            // Act
            var ex = Assert.Throws<TauDbException>(() => inspector.GetColumns());

            // Assert
            Assert.That(ex.Message, Is.EqualTo("Table 'bad_table' does not exist in schema 'zeta'."));
        }

        #endregion

        #region GetPrimaryKey

        // todo:
        // - returns valid primary key
        // - returns valid multi-column primary key
        // - schema not exists => throws
        // - table not exists => throws

        #endregion

        #region GetForeignKeys

        // todo:
        // - returns valid foreign key
        // - returns valid multi-column foreign key
        // - schema not exists => throws
        // - table not exists => throws

        #endregion

        #region GetIndexes

        // todo:
        // - returns valid index
        // - returns valid multi-column index
        // - schema not exists => throws
        // - table not exists => throws

        #endregion

        #region GetTable

        // todo:
        // - returns table with all metadata
        // - schema not exists => throws
        // - table not exists => throws

        #endregion
    }
}
