using NUnit.Framework;
using TauCode.Db.Model.Enums;
using TauCode.Db.Model.Interfaces;
using TauCode.Db.Model.Molds;

namespace TauCode.Db.Tests;

[TestFixture]
public class IndexColumnMoldTests
{
    [Test]
    public void Clone_ValidObject_Clones()
    {
        // Arrange
        IIndexColumnMold indexColumnMold = new IndexColumnMold
        {
            Name = "UserId",
            SortDirection = SortDirection.Descending,
        };

        indexColumnMold.Properties.Add("custom-property", "abc");

        // Act
        var cloned = (IIndexColumnMold)indexColumnMold.Clone(true);

        // Assert
        Assert.That(cloned.Name, Is.EqualTo("UserId"));
        Assert.That(cloned.SortDirection, Is.EqualTo(SortDirection.Descending));

        Assert.That(cloned.Properties, Has.Count.EqualTo(1));
        Assert.That(cloned.Properties["custom-property"], Is.EqualTo("abc"));
    }
}