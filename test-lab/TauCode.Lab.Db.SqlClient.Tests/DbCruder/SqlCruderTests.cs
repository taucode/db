using NUnit.Framework;
using System;

namespace TauCode.Lab.Db.SqlClient.Tests.DbCruder
{
    [TestFixture]
    public class SqlCruderTests : TestBase
    {
        #region Constructor

        [Test]
        public void Constructor_ValidArguments_RunsOk()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void Constructor_SchemaArgumentIsNull_RunsOkWithDboSchema()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void Constructor_ConnectionIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void Constructor_ConnectionIsNotOpen_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region GetTableValuesConverter

        [Test]
        public void GetTableValuesConverter_ValidArgument_ReturnsProperConverter()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetTableValuesConverter_ArgumentIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetTableValuesConverter_NotExistingSchema_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetTableValuesConverter_NotExistingTable_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region ResetTableValuesConverters

        [Test]
        public void ResetTableValuesConverters_NoArguments_RunsOk()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region InsertRow

        [Test]
        public void InsertRow_ValidArguments_InsertsRow()
        {
            // Arrange

            // todo: row is a dictionary, row is a DynamicRow, row is an anon type, row is a strongly-typed dto

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRow_RowIsEmptyAndSelectorIsFalser_InsertsDefaultValues()
        {
            // Arrange

            // todo: EMPTY row is a dictionary, row is a DynamicRow, row is an anon type, row is a strongly-typed dto

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRow_PropertySelectorProducesNoProperties_InsertsDefaultValues()
        {
            // Arrange

            // todo: EMPTY row is a dictionary, row is a DynamicRow, row is an anon type, row is a strongly-typed dto

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRow_PropertySelectorIsNull_UsesAllColumns()
        {
            // Arrange

            // todo: row is a dictionary, row is a DynamicRow, row is an anon type, row is a strongly-typed dto

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRow_RowContainsPropertiesOnWhichSelectorReturnsFalse_RunsOk()
        {
            // Arrange

            // todo: row is a dictionary, row is a DynamicRow, row is an anon type, row is a strongly-typed dto

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRow_NoColumnForSelectedProperty_ThrowsTodo()
        {
            // Arrange

            // todo: row is a dictionary, row is a DynamicRow, row is an anon type, row is a strongly-typed dto

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRow_SchemaDoesNotExist_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRow_TableDoesNotExist_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRow_TableNameIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRow_RowIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRow_RowContainsDBNullValue_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region InsertRows

        [Test]
        public void InsertRows_ValidArguments_InsertsRows()
        {
            // Arrange

            // todo: row is a dictionary, row is a DynamicRow, row is an anon type, row is a strongly-typed dto

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRows_RowsAreEmptyAndSelectorIsFalser_InsertsDefaultValues()
        {
            // Arrange

            // todo: EMPTY row is a dictionary, row is a DynamicRow, row is an anon type, row is a strongly-typed dto

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRows_PropertySelectorProducesNoProperties_InsertsDefaultValues()
        {
            // Arrange

            // todo: EMPTY row is a dictionary, row is a DynamicRow, row is an anon type, row is a strongly-typed dto

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRows_PropertySelectorIsNull_UsesAllColumns()
        {
            // Arrange

            // todo: row is a dictionary, row is a DynamicRow, row is an anon type, row is a strongly-typed dto

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRows_RowsContainPropertiesOnWhichSelectorReturnsFalse_RunsOk()
        {
            // Arrange

            // todo: row is a dictionary, row is a DynamicRow, row is an anon type, row is a strongly-typed dto

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRows_NoColumnForSelectedProperty_ThrowsTodo()
        {
            // Arrange

            // todo: row is a dictionary, row is a DynamicRow, row is an anon type, row is a strongly-typed dto

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRows_NextRowSignatureDiffersFromPrevious_ThrowsTodo()
        {
            // Arrange

            // todo: row is a dictionary, row is a DynamicRow, row is an anon type, row is a strongly-typed dto

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRows_SchemaDoesNotExist_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRows_TableDoesNotExist_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRows_TableNameIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRows_RowsIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRows_RowsContainNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void InsertRows_RowContainsDBNullValue_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
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

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetRow_SelectorIsTruer_DeliversAllColumns()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetRow_SchemaDoesNotExist_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetRow_TableDoesNotExist_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetRow_TableNameIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetRow_IdIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetRow_TableHasNoPrimaryKey_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetRow_TablePrimaryKeyIsMultiColumn_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetRow_IdNotFound_ReturnsNull()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetRow_SelectorIsFalser_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region GetAllRows

        [Test]
        public void GetAllRows_ValidArguments_ReturnsRows()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetAllRows_SelectorIsTruer_DeliversAllColumns()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetAllRows_SchemaDoesNotExist_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetAllRows_TableDoesNotExist_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetAllRows_TableNameIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void GetAllRows_SelectorIsFalser_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
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

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeleteRow_IdNotFound_ReturnsFalse()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeleteRow_SchemaDoesNotExist_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeleteRow_TableDoesNotExist_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeleteRow_TableNameIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeleteRow_IdIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeleteRow_TableHasNoPrimaryKey_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeleteRow_TablePrimaryKeyIsMultiColumn_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        #endregion

        #region DeleteRows

        [Test]
        public void DeleteRows_ValidArguments_DeletesExistingRowsAndReturnsCount()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeleteRows_SchemaDoesNotExist_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeleteRows_TableDoesNotExist_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeleteRows_TableNameIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeleteRows_IdsIsNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeleteRows_IdsContainNull_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeleteRows_TableHasNoPrimaryKey_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        [Test]
        public void DeleteRows_TablePrimaryKeyIsMultiColumn_ThrowsTodo()
        {
            // Arrange

            // Act

            // Assert
            throw new NotImplementedException();
        }

        #endregion
    }
}
