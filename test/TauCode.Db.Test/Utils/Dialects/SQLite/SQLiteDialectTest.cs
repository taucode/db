using NUnit.Framework;
using System;
using System.Linq;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;
using TauCode.Db.Utils.Dialects;
using TauCode.Db.Utils.Dialects.SQLite;

namespace TauCode.Db.Test.Utils.Dialects.SQLite
{
    [TestFixture]
    public class SQLiteDialectTest
    {
        private IDialect _dialect;

        [SetUp]
        public void SetUp()
        {
            _dialect = SQLiteDialect.Instance;
        }

        [Test]
        public void Name_NoArguments_ReturnsValidName()
        {
            // Arrange

            // Act

            // Assert
            Assert.That(_dialect.Name, Is.EqualTo("SQLite"));
        }

        [Test]
        public void ReservedWords_NoArguments_ReturnsValidReservedWords()
        {
            // Arrange

            // Act
            var reservedWords = _dialect.ReservedWords
                .OrderBy(x => x)
                .ToArray();

            // Assert
            var ansiSqlReservedWordsString =

            #region SQLite reserved words

@"
ABORT
ACTION
ADD
AFTER
ALL
ALTER
ANALYZE
AND
AS
ASC
ATTACH
AUTOINCREMENT
BEFORE
BEGIN
BETWEEN
BY
CASCADE
CASE
CAST
CHECK
COLLATE
COLUMN
COMMIT
CONFLICT
CONSTRAINT
CREATE
CROSS
CURRENT
CURRENT_DATE
CURRENT_TIME
CURRENT_TIMESTAMP
DATABASE
DEFAULT
DEFERRABLE
DEFERRED
DELETE
DESC
DETACH
DISTINCT
DO
DROP
EACH
ELSE
END
ESCAPE
EXCEPT
EXCLUSIVE
EXISTS
EXPLAIN
FAIL
FILTER
FOLLOWING
FOR
FOREIGN
FROM
FULL
GLOB
GROUP
HAVING
IF
IGNORE
IMMEDIATE
IN
INDEX
INDEXED
INITIALLY
INNER
INSERT
INSTEAD
INTERSECT
INTO
IS
ISNULL
JOIN
KEY
LEFT
LIKE
LIMIT
MATCH
NATURAL
NO
NOT
NOTHING
NOTNULL
NULL
OF
OFFSET
ON
OR
ORDER
OUTER
OVER
PARTITION
PLAN
PRAGMA
PRECEDING
PRIMARY
QUERY
RAISE
RANGE
RECURSIVE
REFERENCES
REGEXP
REINDEX
RELEASE
RENAME
REPLACE
RESTRICT
RIGHT
ROLLBACK
ROW
ROWS
SAVEPOINT
SELECT
SET
TABLE
TEMP
TEMPORARY
THEN
TO
TRANSACTION
TRIGGER
UNBOUNDED
UNION
UNIQUE
UPDATE
USING
VACUUM
VALUES
VIEW
VIRTUAL
WHEN
WHERE
WINDOW
WITH
WITHOUT";

            #endregion

            var expectedReservedWords = ansiSqlReservedWordsString
                .Split(
                    new string[] { Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries);

            CollectionAssert.AreEqual(
                reservedWords,
                expectedReservedWords
                    .Where(x => x == x.Trim())
                    .OrderBy(x => x)
                    .Distinct()
                    .ToArray());
        }

        [Test]
        public void IdentifierDelimiters_NoArguments_ReturnsValidIdentifierDelimiters()
        {
            // Arrange

            // Act
            var identifierDelimiters = _dialect.IdentifierDelimiters;

            // Assert
            Assert.That(identifierDelimiters.Count, Is.EqualTo(4));

            var tuple = identifierDelimiters[0];
            Assert.That(tuple.Item1, Is.EqualTo('"'));
            Assert.That(tuple.Item2, Is.EqualTo('"'));

            tuple = identifierDelimiters[1];
            Assert.That(tuple.Item1, Is.EqualTo('['));
            Assert.That(tuple.Item2, Is.EqualTo(']'));

            tuple = identifierDelimiters[2];
            Assert.That(tuple.Item1, Is.EqualTo('`'));
            Assert.That(tuple.Item2, Is.EqualTo('`'));

            tuple = identifierDelimiters[3];
            Assert.That(tuple.Item1, Is.EqualTo('\''));
            Assert.That(tuple.Item2, Is.EqualTo('\''));
        }

        [Test]
        public void AcceptableIdentifierFirstChars_NoArguments_ReturnsValidAcceptableIdentifierFirstChars()
        {
            // Arrange

            // Act
            var acceptableIdentifierFirstChars = _dialect.AcceptableIdentifierFirstChars;

            // Assert
            var expectedAcceptableIdentifierFirstChars = new char[]
            {
                #region a-z, A-Z, _

                'a',
                'b',
                'c',
                'd',
                'e',
                'f',
                'g',
                'h',
                'i',
                'j',
                'k',
                'l',
                'm',
                'n',
                'o',
                'p',
                'q',
                'r',
                's',
                't',
                'u',
                'v',
                'w',
                'x',
                'y',
                'z',
                'A',
                'B',
                'C',
                'D',
                'E',
                'F',
                'G',
                'H',
                'I',
                'J',
                'K',
                'L',
                'M',
                'N',
                'O',
                'P',
                'Q',
                'R',
                'S',
                'T',
                'U',
                'V',
                'W',
                'X',
                'Y',
                'Z',
                '_',

                #endregion
            };

            CollectionAssert.AreEqual(
                acceptableIdentifierFirstChars.OrderBy(x => x),
                expectedAcceptableIdentifierFirstChars.OrderBy(x => x));
        }

        [Test]
        public void AcceptableIdentifierInnerChars_NoArguments_ReturnsValidAcceptableIdentifierInnerChars()
        {
            // Arrange

            // Act
            var acceptableIdentifierInnerChars = _dialect.AcceptableIdentifierInnerChars;

            // Assert
            var expectedAcceptableIdentifierInnerChars = new char[]
            {
                #region a-z, A-Z, _

                'a',
                'b',
                'c',
                'd',
                'e',
                'f',
                'g',
                'h',
                'i',
                'j',
                'k',
                'l',
                'm',
                'n',
                'o',
                'p',
                'q',
                'r',
                's',
                't',
                'u',
                'v',
                'w',
                'x',
                'y',
                'z',
                'A',
                'B',
                'C',
                'D',
                'E',
                'F',
                'G',
                'H',
                'I',
                'J',
                'K',
                'L',
                'M',
                'N',
                'O',
                'P',
                'Q',
                'R',
                'S',
                'T',
                'U',
                'V',
                'W',
                'X',
                'Y',
                'Z',
                '_',
                '0',
                '1',
                '2',
                '3',
                '4',
                '5',
                '6',
                '7',
                '8',
                '9',

                #endregion
            };

            CollectionAssert.AreEqual(
                acceptableIdentifierInnerChars.OrderBy(x => x),
                expectedAcceptableIdentifierInnerChars.OrderBy(x => x));
        }

        [Test]
        public void DecorateIdentifier_ValidArguments_ReturnsDecoratedIdentifier()
        {
            // Arrange

            // Act
            var decoratedTable = _dialect.DecorateIdentifier(DbIdentifierType.Table, "the_table", '"');
            var decoratedColumn = _dialect.DecorateIdentifier(DbIdentifierType.Column, "the_column", '[');
            var decoratedIndex = _dialect.DecorateIdentifier(DbIdentifierType.Index, "the_index", '`');
            var decoratedView = _dialect.DecorateIdentifier(DbIdentifierType.View, "the_view", '\'');

            // Assert
            Assert.That(decoratedTable, Is.EqualTo("\"the_table\""));
            Assert.That(decoratedColumn, Is.EqualTo("[the_column]"));
            Assert.That(decoratedIndex, Is.EqualTo("`the_index`"));
            Assert.That(decoratedView, Is.EqualTo("'the_view'"));
        }

        [Test]
        public void DecorateIdentifier_IdentifierIsNull_ThrowsArgumentNullException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentNullException>(() =>
                _dialect.DecorateIdentifier(DbIdentifierType.Table, null, '"'));

            // Assert
            Assert.That(ex.ParamName, Is.EqualTo("identifier"));
        }

        [Test]
        public void DecorateIdentifier_DelimiterIsNull_ReturnsNonDecoratedIdentifier()
        {
            // Arrange

            // Act
            var decorated = _dialect.DecorateIdentifier(DbIdentifierType.Table, "the_table", null);

            // Assert
            Assert.That(decorated, Is.EqualTo("the_table"));
        }

        [Test]
        public void DecoratableTypeIdentifier_NoArguments_ReturnsTrue()
        {
            // Arrange

            // Act
            var decoratable = _dialect.CanDecorateTypeIdentifier;

            // Assert
            Assert.That(decoratable, Is.False);
        }

        [Test]
        public void DecorateIdentifier_InvalidDelimiter_ThrowsArgumentException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentException>(() =>
                _dialect.DecorateIdentifier(DbIdentifierType.Table, "the_table", '('));

            // Assert
            Assert.That(ex.Message, Does.StartWith("Invalid opening delimiter: "));
            Assert.That(ex.ParamName, Is.EqualTo("openingDelimiter"));
        }

        [Test]
        public void DataTypeNames_NoArguments_ReturnsValidValues()
        {
            // Arrange

            // Act
            var dataTypeNames = _dialect.DataTypeNames;

            // Assert
            var expectedDataTypeNamesString =

            #region SQLite data type names

@"
integer
numeric
real
text
blob
";

            #endregion

            var expectedDataTypeNames = expectedDataTypeNamesString
                .Split(
                    new string[] { Environment.NewLine },
                    StringSplitOptions.RemoveEmptyEntries);

            CollectionAssert.AreEqual(
                dataTypeNames,
                expectedDataTypeNames
                    .Where(x => x == x.Trim())
                    .OrderBy(x => x)
                    .Distinct()
                    .ToArray());
        }

        [Test]
        [TestCase("integer", DbTypeFamily.Integer)]
        [TestCase("real", DbTypeFamily.FloatingPointNumber)]
        [TestCase("numeric", DbTypeFamily.PreciseNumber)]
        [TestCase("blob", DbTypeFamily.Binary)]
        [TestCase("text", DbTypeFamily.UnicodeText)]
        public void GetTypeFamily_ValidTypeName_ReturnsTypeFamily(string typeName, DbTypeFamily expectedTypeFamily)
        {
            // Arrange

            // Act
            var typeFamily = _dialect.GetTypeFamily(typeName);

            // Assert
            Assert.That(typeFamily, Is.EqualTo(expectedTypeFamily));
        }

        [Test]
        public void GetTypeFamily_InvalidTypeName_ThrowArgumentException()
        {
            // Arrange

            // Act & Assert
            var ex = Assert.Throws<UnknownDataTypeNameException>(() => _dialect.GetTypeFamily("wrong_type"));

            Assert.That(ex.Message, Does.StartWith("Unknown data type name: 'wrong_type'"));
        }

        [Test]
        public void GetTypeFamily_TypeNameIsNull_ArgumentNullException()
        {
            // Arrange

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => _dialect.GetTypeFamily(null));

            Assert.That(ex.ParamName, Is.EqualTo("typeName"));
        }

        [Test]
        [TestCase("integer", DbTypeNameCategory.SingleWord)]
        [TestCase("real", DbTypeNameCategory.SingleWord)]
        [TestCase("numeric", DbTypeNameCategory.SingleWord)]
        [TestCase("blob", DbTypeNameCategory.SingleWord)]
        [TestCase("text", DbTypeNameCategory.SingleWord)]
        public void GetTypeNameCategory_ValidTypeName_ReturnsTypeNameCategory(string typeName, DbTypeNameCategory expectedTypeNameCategory)
        {
            // Arrange

            // Act
            var category = _dialect.GetTypeNameCategory(typeName);

            // Assert
            Assert.That(category, Is.EqualTo(expectedTypeNameCategory));
        }

        [Test]
        public void GetTypeNameCategory_InvalidTypeName_ThrowArgumentException()
        {
            // Arrange

            // Act & Assert
            var ex = Assert.Throws<UnknownDataTypeNameException>(() => _dialect.GetTypeNameCategory("wrong_type"));

            Assert.That(ex.Message, Does.StartWith("Unknown data type name: 'wrong_type'"));
        }

        [Test]
        public void GetTypeNameCategory_TypeNameIsNull_ArgumentNullException()
        {
            // Arrange

            // Act & Assert
            var ex = Assert.Throws<ArgumentNullException>(() => _dialect.GetTypeNameCategory(null));

            Assert.That(ex.ParamName, Is.EqualTo("typeName"));
        }
    }
}
