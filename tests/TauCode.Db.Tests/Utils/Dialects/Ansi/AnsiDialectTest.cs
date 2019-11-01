using NUnit.Framework;
using System;
using System.Linq;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;
using TauCode.Db.Utils.Dialects;
using TauCode.Db.Utils.Dialects.Ansi;

namespace TauCode.Db.Tests.Utils.Dialects.Ansi
{
    [TestFixture]
    public class AnsiDialectTest
    {
        private IDialect _dialect;

        [SetUp]
        public void SetUp()
        {
            _dialect = AnsiDialect.Instance;
        }

        [Test]
        public void Name_NoArguments_ReturnsValidName()
        {
            // Arrange

            // Act
            IDialect dialect = AnsiDialect.Instance;

            // Assert
            Assert.That(dialect.Name, Is.EqualTo("ANSI SQL"));
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

            #region ANSI SQL reserved words

@"
ADD
ALL
ALLOCATE
ALTER
AND
ANY
ARE
ARRAY
AS
ASENSITIVE
ASYMMETRIC
AT
ATOMIC
AUTHORIZATION
BEGIN
BETWEEN
BIGINT
BINARY
BLOB
BOOLEAN
BOTH
BY
CALL
CALLED
CASCADED
CASE
CAST
CHAR
CHARACTER
CHECK
CLOB
CLOSE
COLLATE
COLUMN
COMMIT
CONDITION
CONNECT
CONSTRAINT
CONTINUE
CORRESPONDING
CREATE
CROSS
CUBE
CURRENT
CURRENT_DATE
CURRENT_DEFAULT_TRANSFORM_GROUP
CURRENT_PATH
CURRENT_ROLE
CURRENT_TIME
CURRENT_TIMESTAMP
CURRENT_TRANSFORM_GROUP_FOR_TYPE
CURRENT_USER
CURSOR
CYCLE
DATE
DAY
DEALLOCATE
DEC
DECIMAL
DECLARE
DEFAULT
DELETE
DEREF
DESCRIBE
DETERMINISTIC
DISCONNECT
DISTINCT
DO
DOUBLE
DROP
DYNAMIC
EACH
ELEMENT
ELSE
ELSEIF
END
ESCAPE
EXCEPT
EXEC
EXECUTE
EXISTS
EXIT
EXTERNAL
FALSE
FETCH
FILTER
FLOAT
FOR
FOREIGN
FREE
FROM
FULL
FUNCTION
GET
GLOBAL
GRANT
GROUP
GROUPING
HANDLER
HAVING
HOLD
HOUR
IDENTITY
IF
IMMEDIATE
IN
INDICATOR
INNER
INOUT
INPUT
INSENSITIVE
INSERT
INT
INTEGER
INTERSECT
INTERVAL
INTO
IS
ITERATE
JOIN
LANGUAGE
LARGE
LATERAL
LEADING
LEAVE
LEFT
LIKE
LOCAL
LOCALTIME
LOCALTIMESTAMP
LOOP
MATCH
MEMBER
MERGE
METHOD
MINUTE
MODIFIES
MODULE
MONTH
MULTISET
NATIONAL
NATURAL
NCHAR
NCLOB
NEW
NO
NONE
NOT
NULL
NUMERIC
OF
OLD
ON
ONLY
OPEN
OR
ORDER
OUT
OUTER
OUTPUT
OVER
OVERLAPS
PARAMETER
PARTITION
PRECISION
PREPARE
PRIMARY
PROCEDURE
RANGE
READS
REAL
RECURSIVE
REF
REFERENCES
REFERENCING
RELEASE
REPEAT
RESIGNAL
RESULT
RETURN
RETURNS
REVOKE
RIGHT
ROLLBACK
ROLLUP
ROW
ROWS
SAVEPOINT
SCOPE
SCROLL
SEARCH
SECOND
SELECT
SENSITIVE
SESSION_USER
SET
SIGNAL
SIMILAR
SMALLINT
SOME
SPECIFIC
SPECIFICTYPE
SQL
SQLEXCEPTION
SQLSTATE
SQLWARNING
START
STATIC
SUBMULTISET
SYMMETRIC
SYSTEM
SYSTEM_USER
TABLE
TABLESAMPLE
THEN
TIME
TIMESTAMP
TIMEZONE_HOUR
TIMEZONE_MINUTE
TO
TRAILING
TRANSLATION
TREAT
TRIGGER
TRUE
UNDO
UNION
UNIQUE
UNKNOWN
UNNEST
UNTIL
UPDATE
USER
USING
VALUE
VALUES
VARCHAR
VARYING
WHEN
WHENEVER
WHERE
WHILE
WINDOW
WITH
WITHIN
WITHOUT
YEAR";

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
            Assert.That(identifierDelimiters.Count, Is.EqualTo(1));

            var tuple = identifierDelimiters.Single();
            Assert.That(tuple.Item1, Is.EqualTo('"'));
            Assert.That(tuple.Item2, Is.EqualTo('"'));
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
            var decorated = _dialect.DecorateIdentifier(DbIdentifierType.Table, "the_table", '"');

            // Assert
            Assert.That(decorated, Is.EqualTo("\"the_table\""));
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
            Assert.That(decoratable, Is.True);
        }

        [Test]
        public void DecorateIdentifier_InvalidDelimiter_ThrowsArgumentException()
        {
            // Arrange

            // Act
            var ex = Assert.Throws<ArgumentException>(() =>
                _dialect.DecorateIdentifier(DbIdentifierType.Table, "the_table", '`'));

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

            #region ANSI SQL data type names

@"
int
integer
bigint
smallint
boolean
double
float
real
date
timestamp
time
array
xml
interval
multiset
binary
char
character
varbinary
varchar
decimal
numeric";

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
        [TestCase("int", DbTypeFamily.Integer)]
        [TestCase("integer", DbTypeFamily.Integer)]
        [TestCase("bigint", DbTypeFamily.Integer)]
        [TestCase("smallint", DbTypeFamily.Integer)]
        [TestCase("boolean", DbTypeFamily.Boolean)]
        [TestCase("double", DbTypeFamily.FloatingPointNumber)]
        [TestCase("float", DbTypeFamily.FloatingPointNumber)]
        [TestCase("real", DbTypeFamily.FloatingPointNumber)]
        [TestCase("date", DbTypeFamily.DateTime)]
        [TestCase("timestamp", DbTypeFamily.DateTime)]
        [TestCase("time", DbTypeFamily.Time)]
        [TestCase("array", DbTypeFamily.Custom)]
        [TestCase("xml", DbTypeFamily.Custom)]
        [TestCase("interval", DbTypeFamily.Custom)]
        [TestCase("multiset", DbTypeFamily.Custom)]
        [TestCase("binary", DbTypeFamily.Binary)]
        [TestCase("char", DbTypeFamily.AnsiText)]
        [TestCase("character", DbTypeFamily.AnsiText)]
        [TestCase("varbinary", DbTypeFamily.Binary)]
        [TestCase("varchar", DbTypeFamily.AnsiText)]
        [TestCase("decimal", DbTypeFamily.PreciseNumber)]
        [TestCase("numeric", DbTypeFamily.PreciseNumber)]
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
        [TestCase("int", DbTypeNameCategory.SingleWord)]
        [TestCase("integer", DbTypeNameCategory.SingleWord)]
        [TestCase("bigint", DbTypeNameCategory.SingleWord)]
        [TestCase("smallint", DbTypeNameCategory.SingleWord)]
        [TestCase("boolean", DbTypeNameCategory.SingleWord)]
        [TestCase("double", DbTypeNameCategory.SingleWord)]
        [TestCase("float", DbTypeNameCategory.SingleWord)]
        [TestCase("real", DbTypeNameCategory.SingleWord)]
        [TestCase("date", DbTypeNameCategory.SingleWord)]
        [TestCase("timestamp", DbTypeNameCategory.SingleWord)]
        [TestCase("time", DbTypeNameCategory.SingleWord)]
        [TestCase("array", DbTypeNameCategory.SingleWord)]
        [TestCase("xml", DbTypeNameCategory.SingleWord)]
        [TestCase("interval", DbTypeNameCategory.SingleWord)]
        [TestCase("multiset", DbTypeNameCategory.SingleWord)]
        [TestCase("binary", DbTypeNameCategory.Sized)]
        [TestCase("char", DbTypeNameCategory.Sized)]
        [TestCase("character", DbTypeNameCategory.Sized)]
        [TestCase("varbinary", DbTypeNameCategory.Sized)]
        [TestCase("varchar", DbTypeNameCategory.Sized)]
        [TestCase("decimal", DbTypeNameCategory.PreciseNumber)]
        [TestCase("numeric", DbTypeNameCategory.PreciseNumber)]
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
