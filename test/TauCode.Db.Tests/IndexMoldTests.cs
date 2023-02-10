using NUnit.Framework;
using TauCode.Db.Model.Enums;
using TauCode.Db.Model.Interfaces;
using TauCode.Db.Model.Molds;
using TauCode.Extensions;

namespace TauCode.Db.Tests;

[TestFixture]
public class IndexMoldTests
{
    [Test]
    public void Clone_ValidObject_Clones()
    {
        // Arrange
        IIndexMold indexMold = new IndexMold
        {
            SchemaName = "dbo",
            TableName = "User",
            Name = "UX_User_Id_Code",
            IsUnique = true,
        };

        indexMold.Columns.AddRangeToCollection(new List<IIndexColumnMold>
        {
            new IndexColumnMold
            {
                Name = "Id",
                SortDirection = SortDirection.Ascending,
            },
            new IndexColumnMold
            {
                Name = "Code",
                SortDirection = SortDirection.Descending,
            },
        });

        indexMold.Columns[0].Properties.Add("p1", "1");
        indexMold.Columns[1].Properties.Add("p1", "2");

        indexMold.Properties.Add("custom-property", "abc");

        // Act
        var clonedIndex = (IIndexMold)indexMold.Clone(true);

        // Assert
        Assert.That(clonedIndex.Name, Is.EqualTo("UX_User_Id_Code"));
        // todo other assertions.
    }
}