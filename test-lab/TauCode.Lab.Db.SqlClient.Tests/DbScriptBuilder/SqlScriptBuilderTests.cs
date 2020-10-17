using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Linq;
using TauCode.Db;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;
using TauCode.Extensions;

namespace TauCode.Lab.Db.SqlClient.Tests.DbScriptBuilder
{
    [TestFixture]
    public class SqlScriptBuilderTests : TestBase
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

        #region Constructor

        [Test]
        public void Constructor_SchemaIsNotNull_RunsOk()
        {
            // Arrange

            // Act
            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab("dbo");

            // Assert
            Assert.That(scriptBuilder.Connection, Is.Null);
            Assert.That(scriptBuilder.Factory, Is.EqualTo(SqlUtilityFactoryLab.Instance));
            Assert.That(scriptBuilder.SchemaName, Is.EqualTo("dbo"));
            Assert.That(scriptBuilder.CurrentOpeningIdentifierDelimiter, Is.EqualTo('['));
        }

        [Test]
        public void Constructor_SchemaIsNull_RunsOk()
        {
            // Arrange

            // Act
            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab(null);

            // Assert
            Assert.That(scriptBuilder.Connection, Is.Null);
            Assert.That(scriptBuilder.Factory, Is.EqualTo(SqlUtilityFactoryLab.Instance));
            Assert.That(scriptBuilder.SchemaName, Is.EqualTo("dbo"));
            Assert.That(scriptBuilder.CurrentOpeningIdentifierDelimiter, Is.EqualTo('['));
        }

        #endregion

        #region CurrentOpeningIdentifierDelimiter

        [Test]
        [TestCase('[')]
        [TestCase('"')]
        [TestCase(null)]
        public void CurrentOpeningIdentifierDelimiter_SetValidValue_ChangesValue(char? openingDelimiter)
        {
            // Arrange
            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab(null);

            // Act
            scriptBuilder.CurrentOpeningIdentifierDelimiter = openingDelimiter;

            // Assert
            Assert.That(scriptBuilder.CurrentOpeningIdentifierDelimiter, Is.EqualTo(openingDelimiter));
        }

        [Test]
        public void CurrentOpeningIdentifierDelimiter_SetInvalidValidValue_ThrowsTodo()
        {
            // Arrange
            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab(null);

            // Act
            var ex = Assert.Throws<TauDbException>(() => scriptBuilder.CurrentOpeningIdentifierDelimiter = '`');

            // Assert
            Assert.That(ex, Has.Message.EqualTo("Invalid opening identifier delimiter: '`'."));
        }

        #endregion

        #region BuildCreateTableScript

        [Test]
        [TestCase('[')]
        [TestCase('"')]
        public void BuildCreateTableScript_IncludeConstraints_ReturnsValidScript(char delimiter)
        {
            // Arrange
            IDbTableInspector tableInspector = new SqlTableInspectorLab(this.Connection, "zeta", "TaxInfo");
            var table = tableInspector.GetTable();

            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab("zeta");
            scriptBuilder.CurrentOpeningIdentifierDelimiter = delimiter;

            string scriptName = "BuildCreateTableScript_Brackets.sql";

            if (delimiter == '"')
            {
                scriptName = "BuildCreateTableScript_DoubleQuotes.sql";
            }

            // Act
            var sql = scriptBuilder.BuildCreateTableScript(table, true);

            var expectedSql = this.GetType().Assembly.GetResourceText(scriptName, true);

            this.Connection.DropTable("zeta", "TaxInfo");
            this.Connection.ExecuteSingleSql(sql);

            IDbTableInspector tableInspector2 = new SqlTableInspectorLab(this.Connection, "zeta", "TaxInfo");
            var table2 = tableInspector2.GetTable();

            var json = JsonConvert.SerializeObject(table);
            var json2 = JsonConvert.SerializeObject(table2);

            // Assert
            TodoCompare(sql, expectedSql);

            Assert.That(sql, Is.EqualTo(expectedSql));
            Assert.That(json, Is.EqualTo(json2));
        }

        [Test]
        [TestCase('[')]
        [TestCase('"')]
        public void BuildCreateTableScript_DoNotIncludeConstraints_ReturnsValidScript(char delimiter)
        {
            // Arrange
            IDbTableInspector tableInspector = new SqlTableInspectorLab(this.Connection, "zeta", "TaxInfo");
            var table = tableInspector.GetTable();

            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab("zeta");
            scriptBuilder.CurrentOpeningIdentifierDelimiter = delimiter;

            string scriptName = "BuildCreateTableScript_Brackets_NoConstraints.sql";

            if (delimiter == '"')
            {
                scriptName = "BuildCreateTableScript_DoubleQuotes_NoConstraints.sql";
            }

            // Act
            var sql = scriptBuilder.BuildCreateTableScript(table, true);

            var expectedSql = this.GetType().Assembly.GetResourceText(scriptName, true);

            this.Connection.DropTable("zeta", "TaxInfo");
            this.Connection.ExecuteSingleSql(sql);

            // Assert
            TodoCompare(sql, expectedSql);

            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        [Test]
        public void BuildCreateTableScript_TableMoldIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab("zeta");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => scriptBuilder.BuildCreateTableScript(null, true));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("table"));
        }

        [Test]
        public void BuildCreateTableScript_TableMoldIsCorrupted_ThrowsArgumentException()
        {
            // Arrange
            IDbTableInspector tableInspector = new SqlTableInspectorLab(this.Connection, "zeta", "HealthInfo");
            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab("zeta");

            var table = tableInspector.GetTable();

            // Act

            // corrupted: table name is null
            var table1 = (TableMold)table.Clone();
            table1.Name = null;
            var ex1 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table1, true));

            // corrupted: columns is null
            var table2 = (TableMold)table.Clone();
            table2.Columns = null;
            var ex2 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table2, true));

            // corrupted: column is null
            var table3 = (TableMold)table.Clone();
            table3.Columns[0] = null;
            var ex3 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table3, true));

            // corrupted: columns is empty
            var table4 = (TableMold)table.Clone();
            table4.Columns.Clear();
            var ex4 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table4, true));

            // corrupted: column name is null
            var table5 = (TableMold)table.Clone();
            table5.Columns[0].Name = null;
            var ex5 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table5, true));

            // corrupted: column type is null
            var table6 = (TableMold)table.Clone();
            table6.Columns[0].Type = null;
            var ex6 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table6, true));

            // corrupted: type name is null
            var table7 = (TableMold)table.Clone();
            table7.Columns[0].Type.Name = null;
            var ex7 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table7, true));

            // corrupted: only type scale is provided
            var table8 = (TableMold)table.Clone();
            table8.Columns[0].Type.Size = null;
            table8.Columns[0].Type.Precision = null;
            table8.Columns[0].Type.Scale = 1;
            var ex8 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table8, true));

            // corrupted: type size and scale are provided
            var table9 = (TableMold)table.Clone();
            table9.Columns[0].Type.Size = 1;
            table9.Columns[0].Type.Precision = null;
            table9.Columns[0].Type.Scale = 1;
            var ex9 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table9, true));

            // corrupted: type size and precision are provided
            var table10 = (TableMold)table.Clone();
            table10.Columns[0].Type.Size = 1;
            table10.Columns[0].Type.Precision = null;
            table10.Columns[0].Type.Scale = 1;
            var ex10 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table10, true));

            // corrupted: type size, precision and scale are provided
            var table11 = (TableMold)table.Clone();
            table11.Columns[0].Type.Size = 1;
            table11.Columns[0].Type.Precision = null;
            table11.Columns[0].Type.Scale = 1;
            var ex11 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table11, true));

            // corrupted: identity seed is null
            var table12 = (TableMold)table.Clone();
            table12.Columns[0].Identity = new ColumnIdentityMold
            {
                Seed = 1.ToString(),
                Increment = 1.ToString(),
            };
            table12.Columns[0].Identity.Seed = null;
            var ex12 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table12, true));

            // corrupted: identity increment is null
            var table13 = (TableMold)table.Clone();
            table13.Columns[0].Identity = new ColumnIdentityMold
            {
                Seed = 1.ToString(),
                Increment = 1.ToString(),
            };
            table13.Columns[0].Identity.Increment = null;
            var ex13 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table13, true));

            // corrupted: pk name is null
            var table14 = (TableMold)table.Clone();
            table14.PrimaryKey.Name = null;
            var ex14 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table14, true));

            // corrupted: pk columns is null
            var table15 = (TableMold)table.Clone();
            table15.PrimaryKey.Columns = null;
            var ex15 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table15, true));

            // corrupted: pk columns is empty
            var table16 = (TableMold)table.Clone();
            table16.PrimaryKey.Columns.Clear();
            var ex16 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table16, true));

            // corrupted: pk columns contain nulls
            var table17 = (TableMold)table.Clone();
            table17.PrimaryKey.Columns[0] = null;
            var ex17 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table17, true));

            // corrupted: fks is null
            var table18 = (TableMold)table.Clone();
            table18.ForeignKeys = null;
            var ex18 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table18, true));

            // corrupted: fks contains nulls
            var table19 = (TableMold)table.Clone();
            table19.ForeignKeys[0] = null;
            var ex19 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table19, true));

            // corrupted: fk name is null
            var table20 = (TableMold)table.Clone();
            table20.ForeignKeys[0].Name = null;
            var ex20 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table20, true));

            // corrupted: fk column names is null
            var table21 = (TableMold)table.Clone();
            table21.ForeignKeys[0].ColumnNames = null;
            var ex21 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table21, true));

            // corrupted: fk column names is empty
            var table22 = (TableMold)table.Clone();
            table22.ForeignKeys[0].ColumnNames.Clear();
            var ex22 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table22, true));

            // corrupted: fk column name is null
            var table23 = (TableMold)table.Clone();
            table23.ForeignKeys[0].ColumnNames[0] = null;
            var ex23 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table23, true));

            // corrupted: fk referenced table name is null
            var table24 = (TableMold)table.Clone();
            table24.ForeignKeys[0].ReferencedTableName = null;
            var ex24 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table24, true));

            // corrupted: fk referenced column names is null
            var table25 = (TableMold)table.Clone();
            table25.ForeignKeys[0].ReferencedColumnNames = null;
            var ex25 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table25, true));

            // corrupted: fk referenced column names contains null
            var table26 = (TableMold)table.Clone();
            table26.ForeignKeys[0].ReferencedColumnNames[0] = null;
            var ex26 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table26, true));

            // corrupted: fk column names and referenced column names have different count
            var table27 = (TableMold)table.Clone();
            table27.ForeignKeys[0].ReferencedColumnNames.Add("extra_column");
            var ex27 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table27, true));

            // corrupted: indexes is null
            var table28 = (TableMold)table.Clone();
            table28.Indexes = null;
            var ex28 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table28, true));

            // corrupted: indexes contain null
            var table29 = (TableMold)table.Clone();
            table29.Indexes[0] = null;
            var ex29 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table29, true));

            // corrupted: index name is null
            var table30 = (TableMold)table.Clone();
            table30.Indexes[0].Name = null;
            var ex30 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table30, true));

            // corrupted: index table name is null
            var table31 = (TableMold)table.Clone();
            table31.Indexes[0].TableName = null;
            var ex31 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table31, true));

            // corrupted: index columns is null
            var table32 = (TableMold)table.Clone();
            table32.Indexes[0].Columns = null;
            var ex32 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table32, true));

            // corrupted: index columns is empty
            var table33 = (TableMold)table.Clone();
            table33.Indexes[0].Columns.Clear();
            var ex33 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table33, true));

            // corrupted: index columns contains null
            var table34 = (TableMold)table.Clone();
            table34.Indexes[0].Columns[0] = null;
            var ex34 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table34, true));

            // corrupted: index column name is null
            var table35 = (TableMold)table.Clone();
            table35.Indexes[0].Columns[0].Name = null;
            var ex35 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateTableScript(table35, true));

            // Assert
            Assert.That(ex1, Has.Message.StartsWith("Table name cannot be null."));
            Assert.That(ex2, Has.Message.StartsWith("Table columns cannot be null."));
            Assert.That(ex3, Has.Message.StartsWith("Table columns cannot contain nulls."));
            Assert.That(ex4, Has.Message.StartsWith("Table columns cannot be empty."));
            Assert.That(ex5, Has.Message.StartsWith("Column name cannot be null."));
            Assert.That(ex6, Has.Message.StartsWith("Column type cannot be null."));
            Assert.That(ex7, Has.Message.StartsWith("Type name cannot be null."));
            Assert.That(ex8, Has.Message.StartsWith("If type scale is provided, precision must be provided as well."));
            Assert.That(ex9, Has.Message.StartsWith("If type size is provided, neither precision nor scale cannot be provided."));
            Assert.That(ex10, Has.Message.StartsWith("If type size is provided, neither precision nor scale cannot be provided."));
            Assert.That(ex11, Has.Message.StartsWith("If type size is provided, neither precision nor scale cannot be provided."));
            Assert.That(ex12, Has.Message.StartsWith("Identity seed cannot be null."));
            Assert.That(ex13, Has.Message.StartsWith("Identity increment cannot be null."));
            Assert.That(ex14, Has.Message.StartsWith("Primary key's name cannot be null."));
            Assert.That(ex15, Has.Message.StartsWith("Primary key's columns cannot be null."));
            Assert.That(ex16, Has.Message.StartsWith("Primary key's columns cannot be empty."));
            Assert.That(ex17, Has.Message.StartsWith("Primary key's columns cannot contain nulls."));
            Assert.That(ex18, Has.Message.StartsWith("Table foreign keys list cannot be null."));
            Assert.That(ex19, Has.Message.StartsWith("Table foreign keys cannot contain nulls."));
            Assert.That(ex20, Has.Message.StartsWith("Foreign key name cannot be null."));
            Assert.That(ex21, Has.Message.StartsWith("Foreign key column names collection cannot be null."));
            Assert.That(ex22, Has.Message.StartsWith("Foreign key column names collection cannot be empty."));
            Assert.That(ex23, Has.Message.StartsWith("Foreign key column names cannot contain nulls."));
            Assert.That(ex24, Has.Message.StartsWith("Foreign key's referenced table name cannot be null."));
            Assert.That(ex25, Has.Message.StartsWith("Foreign key referenced column names collection cannot be null."));
            Assert.That(ex26, Has.Message.StartsWith("Foreign key referenced column names cannot contain nulls."));
            Assert.That(ex27, Has.Message.StartsWith("Foreign key's column name count does not match referenced column name count."));
            Assert.That(ex28, Has.Message.StartsWith("Table indexes list cannot be null."));
            Assert.That(ex29, Has.Message.StartsWith("Table indexes cannot contain nulls."));
            Assert.That(ex30, Has.Message.StartsWith("Index name cannot be null."));
            Assert.That(ex31, Has.Message.StartsWith("Index table name cannot be null."));
            Assert.That(ex32, Has.Message.StartsWith("Index columns cannot be null."));
            Assert.That(ex33, Has.Message.StartsWith("Index columns cannot be empty."));
            Assert.That(ex34, Has.Message.StartsWith("Index columns cannot contain nulls."));
            Assert.That(ex35, Has.Message.StartsWith("Index column name cannot be null."));

            #region check arg name

            ArgumentException[] exceptions = new[]
            {
                ex1,
                ex2,
                ex3,
                ex4,
                ex5,
                ex6,
                ex7,
                ex8,
                ex9,
                ex10,
                ex11,
                ex12,
                ex13,
                ex14,
                ex15,
                ex16,
                ex17,
                ex18,
                ex19,
                ex20,
                ex21,
                ex22,
                ex23,
                ex24,
                ex25,
                ex26,
                ex27,
                ex28,
                ex29,
                ex30,
                ex31,
                ex32,
                ex33,
                ex34,
                ex35,
            };

            Assert.That(exceptions.All(x => x.ParamName == "table"), Is.True);

            #endregion
        }

        #endregion

        #region BuildCreateIndexScript

        [Test]
        [TestCase('[')]
        [TestCase('"')]
        public void BuildCreateIndexScript_UniqueIndex_ReturnsValidScript(char delimiter)
        {
            // Arrange
            IDbTableInspector tableInspector = new SqlTableInspectorLab(this.Connection, "zeta", "WorkInfo");
            var table = tableInspector.GetTable();
            var index = table.Indexes.Single(x => x.Name == "UX_workInfo_Hash");

            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab("zeta")
            {
                CurrentOpeningIdentifierDelimiter = delimiter,
            };

            // Act
            var sql = scriptBuilder.BuildCreateIndexScript(index);

            // Assert
            var expectedSql = this.GetType().Assembly.GetResourceText("BuildCreateIndexScript_UniqueIndex_Brackets.sql", true);

            if (delimiter == '"')
            {
                expectedSql = this.GetType().Assembly.GetResourceText("BuildCreateIndexScript_UniqueIndex_DoubleQuotes.sql", true);
            }

            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        [Test]
        [TestCase('[')]
        [TestCase('"')]
        public void BuildCreateIndexScript_NonUniqueIndex_ReturnsValidScript(char delimiter)
        {
            // Arrange
            IDbTableInspector tableInspector = new SqlTableInspectorLab(this.Connection, "zeta", "HealthInfo");
            var table = tableInspector.GetTable();
            var index = table.Indexes.Single(x => x.Name == "IX_healthInfo_metricAmetricB");

            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab("zeta")
            {
                CurrentOpeningIdentifierDelimiter = delimiter,
            };

            // Act
            var sql = scriptBuilder.BuildCreateIndexScript(index);

            // Assert
            var expectedSql = this.GetType().Assembly.GetResourceText("BuildCreateIndexScript_NonUniqueIndex_Brackets.sql", true);

            if (delimiter == '"')
            {
                expectedSql = this.GetType().Assembly.GetResourceText("BuildCreateIndexScript_NonUniqueIndex_DoubleQuotes.sql", true);
            }

            Assert.That(sql, Is.EqualTo(expectedSql));
        }

        [Test]
        public void BuildCreateIndexScript_IndexIsNull_ThrowsArgumentNullException()
        {
            // Arrange
            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab("zeta");

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() => scriptBuilder.BuildCreateIndexScript(null));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("index"));
        }

        [Test]
        public void BuildCreateIndexScript_IndexIsCorrupted_ThrowsArgumentException()
        {
            // Arrange
            IDbTableInspector tableInspector = new SqlTableInspectorLab(this.Connection, "zeta", "HealthInfo");
            var table = tableInspector.GetTable();
            var index = table.Indexes.Single(x => x.Name == "IX_healthInfo_metricAmetricB");

            IDbScriptBuilder scriptBuilder = new SqlScriptBuilderLab("zeta");

            // Act
            // corrupted: index name is null
            var index1 = (IndexMold)index.Clone();
            index1.Name = null;
            var ex1 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateIndexScript(index1));

            // corrupted: index index name is null
            var index2 = (IndexMold)index.Clone();
            index2.TableName = null;
            var ex2 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateIndexScript(index2));

            // corrupted: index columns is null
            var index3 = (IndexMold)index.Clone();
            index3.Columns = null;
            var ex3 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateIndexScript(index3));

            // corrupted: index columns is empty
            var index4 = (IndexMold)index.Clone();
            index4.Columns.Clear();
            var ex4 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateIndexScript(index4));

            // corrupted: index columns contains null
            var index5 = (IndexMold)index.Clone();
            index5.Columns[0] = null;
            var ex5 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateIndexScript(index5));

            // corrupted: index column name is null
            var index6 = (IndexMold)index.Clone();
            index6.Columns[0].Name = null;
            var ex6 = Assert.Throws<ArgumentException>(() => scriptBuilder.BuildCreateIndexScript(index6));

            // Assert
            Assert.That(ex1, Has.Message.StartsWith("Index name cannot be null."));
            Assert.That(ex2, Has.Message.StartsWith("Index table name cannot be null."));
            Assert.That(ex3, Has.Message.StartsWith("Index columns cannot be null."));
            Assert.That(ex4, Has.Message.StartsWith("Index columns cannot be empty."));
            Assert.That(ex5, Has.Message.StartsWith("Index columns cannot contain nulls."));
            Assert.That(ex6, Has.Message.StartsWith("Index column name cannot be null."));
        }

        #endregion

        #region BuildDropTableScript

        // todo
        // - happy path
        // - respects delimiter
        // - arg is null => throws

        #endregion

        #region BuildInsertScript

        // todo
        // - happy path
        // - happy path: columnToParameterMappings is empty (=> default values)
        // - columnToParameterMappings has unknown columns => throws
        // - table is null => throws
        // - table is corrupted => throws
        // - columnToParameterMappings is null => throws
        // - columnToParameterMappings is corrupted => throws
        // - respects delimiter

        #endregion

        #region BuildUpdateScript

        // todo
        // - happy path
        // - columnToParameterMappings is empty => throws
        // - no pk => throws
        // - PK is multi-column => throws
        // - columnToParameterMappings does not contain PK column => throws
        // - table is null => throws
        // - table is corrupted => throws
        // - columnToParameterMappings is null => throws
        // - columnToParameterMappings is corrupted => throws
        // - respects delimiter

        #endregion

        #region BuildSelectByPrimaryKeyScript

        // todo
        // - happy path : respects columnSelector
        // - columnSelector is null => delivers all
        // - pk is absent => throws
        // - pk is multi-column => throws
        // - play around with columnSelector
        // - columnSelector returned 0 columns => throws
        // - respects delimiter

        #endregion

        #region BuildSelectAllScript

        // todo

        #endregion

        #region BuildDeleteByIdScript

        // todo

        #endregion

        #region BuildDeleteScript

        // todo

        #endregion
    }
}
