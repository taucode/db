using NUnit.Framework;
using System;
using System.Linq;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;
using TauCode.Db.Utils.Dialects;
using TauCode.Db.Utils.Dialects.SqlServerCe;

namespace TauCode.Db.Test.Utils.Dialects.SqlServerCe
{
    [TestFixture]
    public class SqlServerCeDialectTest
    {
        private IDialect _dialect;

        [SetUp]
        public void SetUp()
        {
            _dialect = SqlServerCeDialect.Instance;
        }

        [Test]
        public void Name_NoArguments_ReturnsValidName()
        {
            // Arrange

            // Act
            
            // Assert
            Assert.That(_dialect.Name, Is.EqualTo("SQL Server Compact"));
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

            #region SQL Server CE reserved words

@"
@@IDENTITY
ADD
ALL
ALTER
AND
ANY
AS
ASC
AUTHORIZATION
AVG
BACKUP
BEGIN
BETWEEN
BREAK
BROWSE
BULK
BY
CASCADE
CASE
CHECK
CHECKPOINT
CLOSE
CLUSTERED
COALESCE
COLLATE
COLUMN
COMMIT
COMPUTE
CONSTRAINT
CONTAINS
CONTAINSTABLE
CONTINUE
CONVERT
COUNT
CREATE
CROSS
CURRENT
CURRENT_DATE
CURRENT_TIME
CURRENT_TIMESTAMP
CURRENT_USER
CURSOR
DATABASE
DATABASEPASSWORD
DATEADD
DATEDIFF
DATENAME
DATEPART
DBCC
DEALLOCATE
DECLARE
DEFAULT
DELETE
DENY
DESC
DISK
DISTINCT
DISTRIBUTED
DOUBLE
DROP
DUMP
ELSE
ENCRYPTION
END
ERRLVL
ESCAPE
EXCEPT
EXEC
EXECUTE
EXISTS
EXIT
EXPRESSION
FETCH
FILE
FILLFACTOR
FOR
FOREIGN
FREETEXT
FREETEXTTABLE
FROM
FULL
FUNCTION
GOTO
GRANT
GROUP
HAVING
HOLDLOCK
IDENTITY
IDENTITY_INSERT
IDENTITYCOL
IF
IN
INDEX
INNER
INSERT
INTERSECT
INTO
IS
JOIN
KEY
KILL
LEFT
LIKE
LINENO
LOAD
MAX
MIN
NATIONAL
NOCHECK
NONCLUSTERED
NOT
NULL
NULLIF
OF
OFF
OFFSETS
ON
OPEN
OPENDATASOURCE
OPENQUERY
OPENROWSET
OPENXML
OPTION
OR
ORDER
OUTER
OVER
PERCENT
PLAN
PRECISION
PRIMARY
PRINT
PROC
PROCEDURE
PUBLIC
RAISERROR
READ
READTEXT
RECONFIGURE
REFERENCES
REPLICATION
RESTORE
RESTRICT
RETURN
REVOKE
RIGHT
ROLLBACK
ROWCOUNT
ROWGUIDCOL
RULE
SAVE
SCHEMA
SELECT
SESSION_USER
SET
SETUSER
SHUTDOWN
SOME
STATISTICS
SUM
SYSTEM_USER
TABLE
TEXTSIZE
THEN
TO
TOP
TRAN
TRANSACTION
TRIGGER
TRUNCATE
TSEQUAL
UNION
UNIQUE
UPDATE
UPDATETEXT
USE
USER
VALUES
VARYING
VIEW
WAITFOR
WHEN
WHERE
WHILE
WITH
WRITETEXT";

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
            Assert.That(identifierDelimiters.Count, Is.EqualTo(2));

            var tuple = identifierDelimiters[0];
            Assert.That(tuple.Item1, Is.EqualTo('['));
            Assert.That(tuple.Item2, Is.EqualTo(']'));

            tuple = identifierDelimiters[1];
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
            var decoratedTable = _dialect.DecorateIdentifier(DbIdentifierType.Table, "the_table", '"');
            var decoratedColumn = _dialect.DecorateIdentifier(DbIdentifierType.Column, "the_column", '[');

            // Assert
            Assert.That(decoratedTable, Is.EqualTo("\"the_table\""));
            Assert.That(decoratedColumn, Is.EqualTo("[the_column]"));
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

            #region SQL Server CE type names

@"
uniqueidentifier
bigint
int
integer
smallint
tinyint
bit
datetime
float
real
image
money
ntext
binary
varbinary
nchar
nvarchar
dec
decimal
numeric
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
        [TestCase("uniqueidentifier", DbTypeFamily.Uniqueidentifier)]
        [TestCase("bigint", DbTypeFamily.Integer)]
        [TestCase("int", DbTypeFamily.Integer)]
        [TestCase("integer", DbTypeFamily.Integer)]
        [TestCase("smallint", DbTypeFamily.Integer)]
        [TestCase("tinyint", DbTypeFamily.Integer)]
        [TestCase("bit", DbTypeFamily.Boolean)]
        [TestCase("datetime", DbTypeFamily.DateTime)]
        [TestCase("float", DbTypeFamily.FloatingPointNumber)]
        [TestCase("real", DbTypeFamily.FloatingPointNumber)]
        [TestCase("image", DbTypeFamily.Binary)]
        [TestCase("money", DbTypeFamily.PreciseNumber)]
        [TestCase("ntext", DbTypeFamily.UnicodeText)]
        [TestCase("binary", DbTypeFamily.Binary)]
        [TestCase("varbinary", DbTypeFamily.Binary)]
        [TestCase("nchar", DbTypeFamily.UnicodeText)]
        [TestCase("nvarchar", DbTypeFamily.UnicodeText)]
        [TestCase("dec", DbTypeFamily.PreciseNumber)]
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
        [TestCase("uniqueidentifier", DbTypeNameCategory.SingleWord)]
        [TestCase("bigint", DbTypeNameCategory.SingleWord)]
        [TestCase("int", DbTypeNameCategory.SingleWord)]
        [TestCase("integer", DbTypeNameCategory.SingleWord)]
        [TestCase("smallint", DbTypeNameCategory.SingleWord)]
        [TestCase("tinyint", DbTypeNameCategory.SingleWord)]
        [TestCase("bit", DbTypeNameCategory.SingleWord)]
        [TestCase("datetime", DbTypeNameCategory.SingleWord)]
        [TestCase("float", DbTypeNameCategory.SingleWord)]
        [TestCase("real", DbTypeNameCategory.SingleWord)]
        [TestCase("image", DbTypeNameCategory.SingleWord)]
        [TestCase("money", DbTypeNameCategory.SingleWord)]
        [TestCase("ntext", DbTypeNameCategory.SingleWord)]
        [TestCase("binary", DbTypeNameCategory.Sized)]
        [TestCase("varbinary", DbTypeNameCategory.Sized)]
        [TestCase("nchar", DbTypeNameCategory.Sized)]
        [TestCase("nvarchar", DbTypeNameCategory.Sized)]
        [TestCase("dec", DbTypeNameCategory.PreciseNumber)]
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
