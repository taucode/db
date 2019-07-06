using NUnit.Framework;
using TauCode.Db.Utils.Parsing.Core;

namespace TauCode.Db.Test.Utils.Parsing
{
    [TestFixture]
    public class PositionTest
    {
        [Test]
        [TestCase(10, 20)]
        [TestCase(0, 0)]
        [TestCase(-100, -300)]
        public void Constructor_AnyArguments_PositionCreated(int line, int column)
        {
            // Arrange
            
            // Act
            var pos = new Position(line, column);

            // Assert
            Assert.That(pos.Line, Is.EqualTo(line));
            Assert.That(pos.Column, Is.EqualTo(column));
        }

        [Test]
        [TestCase(10, 20, 10, 21, false)]
        [TestCase(1, 2, 1, 2, true)]
        public void EqualsPosition_ArgumentsProvided_ReturnsValidResult(
            int line1,
            int column1,
            int line2,
            int column2,
            bool expectedResult)
        {
            // Arrange
            var pos1 = new Position(line1, column1);
            var pos2 = new Position(line2, column2);

            // Act
            var result = pos1.Equals(pos2);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
            Assert.That(pos1.GetHashCode() == pos2.GetHashCode(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(2, 10, 1, 20, +1)]
        [TestCase(2, 10, 3, 1, -1)]
        [TestCase(100, 10, 100, 5, +1)]
        [TestCase(100, 3, 100, 4, -1)]
        [TestCase(-5, 77, -6, 77, +1)]
        [TestCase(-5, 13, -4, 13, -1)]
        [TestCase(15, 99, 15, 99, 0)]
        public void CompareTo_ArgumentsProvided_ReturnsValidResult(
            int line1,
            int column1,
            int line2,
            int column2,
            int expectedResult)
        {
            // Arrange
            var pos1 = new Position(line1, column1);
            var pos2 = new Position(line2, column2);

            // Act
            var result = pos1.CompareTo(pos2);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(10, 20, 10, 21, false)]
        [TestCase(1, 2, 1, 2, true)]
        public void Equals_ArgumentsProvided_ReturnsValidResult(
            int line1,
            int column1,
            int line2,
            int column2,
            bool expectedResult)
        {
            // Arrange
            var pos1 = new Position(line1, column1);
            object o1 = pos1;
            var pos2 = new Position(line2, column2);
            object o2 = pos2;

            // Act
            var result = o1.Equals(o2);

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
            Assert.That(o1.GetHashCode() == o2.GetHashCode(), Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(2, 10, 1, 20, true)]
        [TestCase(2, 10, 3, 1, false)]
        [TestCase(100, 10, 100, 5, true)]
        [TestCase(100, 3, 100, 4, false)]
        [TestCase(-5, 77, -6, 77, true)]
        [TestCase(-5, 13, -4, 13, false)]
        [TestCase(15, 99, 15, 99, false)]
        public void OperatorGreater_ArgumentsProvided_ReturnsValidResult(
            int line1,
            int column1,
            int line2,
            int column2,
            bool expectedResult)
        {
            // Arrange
            var pos1 = new Position(line1, column1);
            var pos2 = new Position(line2, column2);

            // Act
            var result = pos1 > pos2;

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(2, 10, 1, 20, true)]
        [TestCase(2, 10, 3, 1, false)]
        [TestCase(100, 10, 100, 5, true)]
        [TestCase(100, 3, 100, 4, false)]
        [TestCase(-5, 77, -6, 77, true)]
        [TestCase(-5, 13, -4, 13, false)]
        [TestCase(15, 99, 15, 99, true)]
        public void OperatorGreaterOrEqual_ArgumentsProvided_ReturnsValidResult(
            int line1,
            int column1,
            int line2,
            int column2,
            bool expectedResult)
        {
            // Arrange
            var pos1 = new Position(line1, column1);
            var pos2 = new Position(line2, column2);

            // Act
            var result = pos1 >= pos2;

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(2, 10, 1, 20, false)]
        [TestCase(2, 10, 3, 1, true)]
        [TestCase(100, 10, 100, 5, false)]
        [TestCase(100, 3, 100, 4, true)]
        [TestCase(-5, 77, -6, 77, false)]
        [TestCase(-5, 13, -4, 13, true)]
        [TestCase(15, 99, 15, 99, false)]
        public void OperatorLess_ArgumentsProvided_ReturnsValidResult(
            int line1,
            int column1,
            int line2,
            int column2,
            bool expectedResult)
        {
            // Arrange
            var pos1 = new Position(line1, column1);
            var pos2 = new Position(line2, column2);

            // Act
            var result = pos1 < pos2;

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(2, 10, 1, 20, false)]
        [TestCase(2, 10, 3, 1, true)]
        [TestCase(100, 10, 100, 5, false)]
        [TestCase(100, 3, 100, 4, true)]
        [TestCase(-5, 77, -6, 77, false)]
        [TestCase(-5, 13, -4, 13, true)]
        [TestCase(15, 99, 15, 99, true)]
        public void OperatorLessOrEqual_ArgumentsProvided_ReturnsValidResult(
            int line1,
            int column1,
            int line2,
            int column2,
            bool expectedResult)
        {
            // Arrange
            var pos1 = new Position(line1, column1);
            var pos2 = new Position(line2, column2);

            // Act
            var result = pos1 <= pos2;

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }

        [Test]
        [TestCase(2, 10, 1, 20, false)]
        [TestCase(2, 10, 3, 1, false)]
        [TestCase(100, 10, 100, 5, false)]
        [TestCase(100, 3, 100, 4, false)]
        [TestCase(-5, 77, -6, 77, false)]
        [TestCase(-5, 13, -4, 13, false)]
        [TestCase(15, 99, 15, 99, true)]
        public void OperatorEqual_ArgumentsProvided_ReturnsValidResult(
            int line1,
            int column1,
            int line2,
            int column2,
            bool expectedResult)
        {
            // Arrange
            var pos1 = new Position(line1, column1);
            object o1 = pos1;
            var pos2 = new Position(line2, column2);
            object o2 = pos2;
            

            // Act
            var operatorResult = pos1 == pos2;
            var equalsPositionsResult = Equals(pos1, pos2);
            var equalsObjectsResult = Equals(o1, o2);
            var boxedEqualsResult = o1 == o2;

            // Assert
            Assert.That(operatorResult, Is.EqualTo(expectedResult));
            Assert.That(equalsPositionsResult, Is.EqualTo(expectedResult));
            Assert.That(equalsObjectsResult, Is.EqualTo(expectedResult));
            Assert.That(boxedEqualsResult, Is.False);
        }

        [Test]
        [TestCase(2, 10, 1, 20, true)]
        [TestCase(2, 10, 3, 1, true)]
        [TestCase(100, 10, 100, 5, true)]
        [TestCase(100, 3, 100, 4, true)]
        [TestCase(-5, 77, -6, 77, true)]
        [TestCase(-5, 13, -4, 13, true)]
        [TestCase(15, 99, 15, 99, false)]
        public void OperatorNotEqual_ArgumentsProvided_ReturnsValidResult(
            int line1,
            int column1,
            int line2,
            int column2,
            bool expectedResult)
        {
            // Arrange
            var pos1 = new Position(line1, column1);
            var pos2 = new Position(line2, column2);

            // Act
            var result = pos1 != pos2;

            // Assert
            Assert.That(result, Is.EqualTo(expectedResult));
        }
    }
}
