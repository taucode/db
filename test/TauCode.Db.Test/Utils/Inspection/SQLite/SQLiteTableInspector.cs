﻿using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db.Model;
using TauCode.Db.Utils.Inspection;
using TauCode.Db.Utils.Inspection.SQLite;

namespace TauCode.Db.Test.Utils.Inspection.SQLite
{
    public class SQLiteTableInspector : SQLiteTestBase
    {
        private IDbInspector _dbInspector;
        private ITableInspector _tableInspector;

        [SetUp]
        public void SetUp()
        {
            _dbInspector = new SQLiteInspector(this.Connection);
            _tableInspector = _dbInspector.GetTableInspector("client");
        }

        [Test]
        public void TableName_NoArguments_ReturnsTableName()
        {
            // Arrange

            // Act
            var tableName = _tableInspector.TableName;

            // Assert
            Assert.That(tableName, Is.EqualTo("client"));
        }

        [Test]
        public void GetColumnMolds_NoArguments_ReturnsColumnMolds()
        {
            // Arrange

            // Act
            var columns = _tableInspector.GetColumnMolds();

            // Assert
            this.AssertClientTableColumns(columns);
        }

        [Test]
        public void GetPrimaryKeyMold_NoArguments_ReturnsPrimaryKeyMold()
        {
            // Arrange
            var tableInspector = _dbInspector.GetTableInspector("secret");

            // Act
            var primaryKeyMold = tableInspector.GetPrimaryKeyMold();

            // Assert
            Assert.That(primaryKeyMold, Is.Not.Null);
            Assert.That(primaryKeyMold.Name, Is.EqualTo("PK_secret"));
            Assert.That(primaryKeyMold.ColumnNames, Has.Count.EqualTo(2));
            Assert.That(primaryKeyMold.ColumnNames[0], Is.EqualTo("id_base"));
            Assert.That(primaryKeyMold.ColumnNames[1], Is.EqualTo("id_value"));
        }

        [Test]
        public void GetForeignKeyMolds_NoArguments_ReturnsForeignKeyMolds()
        {
            // Arrange
            var tableInspector = _dbInspector.GetTableInspector("secret_detail");

            // Act
            var foreignKeyMolds = tableInspector.GetForeignKeyMolds();

            // Assert
            Assert.That(foreignKeyMolds, Has.Count.EqualTo(2));

            var foreignKey = foreignKeyMolds.Single(x => x.Name == "FK_secretDetail_secret");
            Assert.That(foreignKey.ReferencedTableName, Is.EqualTo("secret"));
            CollectionAssert.AreEqual(foreignKey.ColumnNames, new[] { "secret_id_base", "secret_id_value" });
            CollectionAssert.AreEqual(foreignKey.ReferencedColumnNames, new[] { "id_base", "id_value" });

            foreignKey = foreignKeyMolds.Single(x => x.Name == "FK_secretDetail_color");
            Assert.That(foreignKey.ReferencedTableName, Is.EqualTo("color"));
            CollectionAssert.AreEqual(foreignKey.ColumnNames, new[] { "color_id" });
            CollectionAssert.AreEqual(foreignKey.ReferencedColumnNames, new[] { "id" });
        }

        [Test]
        public void GetIndexMolds_NoArguments_ReturnsIndexMolds()
        {
            // Arrange
            var tableInspector = _dbInspector.GetTableInspector("secret");

            // Act
            var indexMolds = tableInspector.GetIndexMolds();

            // Assert
            Assert.That(indexMolds, Has.Count.EqualTo(2));

            var index = indexMolds.Single(x => x.Name == "IX_secret_name");
            Assert.That(index.IsUnique, Is.False);
            Assert.That(index.TableName, Is.EqualTo("secret"));
            CollectionAssert.AreEqual(index.ColumnNames, new[] { "name" });

            index = indexMolds.Single(x => x.Name == "UX_secret_keyStart_keyEnd");
            Assert.That(index.IsUnique, Is.True);
            Assert.That(index.TableName, Is.EqualTo("secret"));
            CollectionAssert.AreEqual(index.ColumnNames, new[] { "key_start", "key_end" });
        }

        [Test]
        public void GetTableMold_NoArguments_ReturnsTableMold()
        {
            // Arrange

            // Act
            var table = _tableInspector.GetTableMold();

            // Assert
            Assert.That(table.Name, Is.EqualTo("client"));

            Assert.That(table.PrimaryKey, Is.Not.Null);
            Assert.That(table.PrimaryKey.Name, Is.EqualTo("PK_client"));
            Assert.That(table.PrimaryKey.ColumnNames, Has.Count.EqualTo(1));

            this.AssertClientTableColumns(table.Columns);
        }

        #region Helper Methods

        private void AssertClientTableColumns(List<ColumnMold> columns)
        {
            Assert.That(columns, Has.Count.EqualTo(23));

            var column = columns[0];
            Assert.That(column.Name, Is.EqualTo("id"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("integer"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("integer"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.False);
            Assert.That(column.Identity, Is.Not.Null);

            column = columns[1];
            Assert.That(column.Name, Is.EqualTo("login"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("text"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("text"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.False);
            Assert.That(column.Identity, Is.Null);

            column = columns[2];
            Assert.That(column.Name, Is.EqualTo("email"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("text"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("text"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.False);
            Assert.That(column.Identity, Is.Null);

            column = columns[3];
            Assert.That(column.Name, Is.EqualTo("password_hash"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("text"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("text"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.False);
            Assert.That(column.Identity, Is.Null);

            column = columns[4];
            Assert.That(column.Name, Is.EqualTo("picture"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("blob"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("blob"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.True);
            Assert.That(column.Identity, Is.Null);

            column = columns[5];
            Assert.That(column.Name, Is.EqualTo("birthday"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("integer"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("integer"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.True);
            Assert.That(column.Identity, Is.Null);

            column = columns[6];
            Assert.That(column.Name, Is.EqualTo("gender"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("integer"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("integer"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.False);
            Assert.That(column.Identity, Is.Null);

            column = columns[7];
            Assert.That(column.Name, Is.EqualTo("rating"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("numeric"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("numeric"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.True);
            Assert.That(column.Identity, Is.Null);

            column = columns[8];
            Assert.That(column.Name, Is.EqualTo("fortune"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("numeric"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("numeric"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.False);
            Assert.That(column.Identity, Is.Null);

            column = columns[9];
            Assert.That(column.Name, Is.EqualTo("signature"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("blob"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("blob"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.False);
            Assert.That(column.Identity, Is.Null);

            column = columns[10];
            Assert.That(column.Name, Is.EqualTo("visual_age"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("integer"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("integer"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.True);
            Assert.That(column.Identity, Is.Null);

            column = columns[11];
            Assert.That(column.Name, Is.EqualTo("alternate_rate"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("numeric"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("numeric"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.True);
            Assert.That(column.Identity, Is.Null);

            column = columns[12];
            Assert.That(column.Name, Is.EqualTo("iq_index"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("real"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("real"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.False);
            Assert.That(column.Identity, Is.Null);

            column = columns[13];
            Assert.That(column.Name, Is.EqualTo("index16"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("integer"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("integer"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.True);
            Assert.That(column.Identity, Is.Null);

            column = columns[14];
            Assert.That(column.Name, Is.EqualTo("index32"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("integer"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("integer"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.False);
            Assert.That(column.Identity, Is.Null);

            column = columns[15];
            Assert.That(column.Name, Is.EqualTo("index64"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("integer"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("integer"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.True);
            Assert.That(column.Identity, Is.Null);

            column = columns[16];
            Assert.That(column.Name, Is.EqualTo("the_real"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("real"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("real"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.False);
            Assert.That(column.Identity, Is.Null);

            column = columns[17];
            Assert.That(column.Name, Is.EqualTo("guid"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("blob"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("blob"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.False);
            Assert.That(column.Identity, Is.Null);

            column = columns[18];
            Assert.That(column.Name, Is.EqualTo("ansi_char"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("text"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("text"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.True);
            Assert.That(column.Identity, Is.Null);

            column = columns[19];
            Assert.That(column.Name, Is.EqualTo("ansi_varchar"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("text"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("text"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.True);
            Assert.That(column.Identity, Is.Null);

            column = columns[20];
            Assert.That(column.Name, Is.EqualTo("ansi_text"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("text"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("text"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.True);
            Assert.That(column.Identity, Is.Null);

            column = columns[21];
            Assert.That(column.Name, Is.EqualTo("unicode_text"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("text"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("text"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.True);
            Assert.That(column.Identity, Is.Null);

            column = columns[22];
            Assert.That(column.Name, Is.EqualTo("fingerprint"));
            Assert.That(column.Type, Is.Not.Null);
            Assert.That(column.Type.Name, Is.EqualTo("blob"));
            Assert.That(column.Type.GetDefaultDefinition(), Is.EqualTo("blob"));
            Assert.That(column.Type.Size, Is.Null);
            Assert.That(column.Type.Precision, Is.Null);
            Assert.That(column.Type.Scale, Is.Null);
            Assert.That(column.IsNullable, Is.True);
            Assert.That(column.Identity, Is.Null);
        }

        #endregion
    }
}