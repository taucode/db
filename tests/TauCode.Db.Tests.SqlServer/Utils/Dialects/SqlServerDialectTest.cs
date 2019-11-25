using System;
using System.Linq;
using NUnit.Framework;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;
using TauCode.Db.Utils.Dialects;
using TauCode.Db.Utils.Dialects.SqlServer;

namespace TauCode.Db.Tests.SqlServer.Utils.Dialects
{
    [TestFixture]
    public class SqlServerDialectTest
    {
        private IDialect _dialect;

        [SetUp]
        public void SetUp()
        {
            _dialect = SqlServerDialect.Instance;
        }

        [Test]
        public void Name_NoArguments_ReturnsValidName()
        {
            // Arrange

            // Act

            // Assert
            Assert.That(_dialect.Name, Is.EqualTo("SQL Server"));
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

            #region SQL Server reserved words

@"
ABSOLUTE
ACTION
ADA
ADD
ADMIN
AFTER
AGGREGATE
ALIAS
ALL
ALLOCATE
ALTER
AND
ANY
ARE
ARRAY
AS
ASC
ASENSITIVE
ASSERTION
ASYMMETRIC
AT
ATOMIC
AUTHORIZATION
AVG
BACKUP
BEFORE
BEGIN
BETWEEN
BINARY
BIT
BIT_LENGTH
BLOB
BOOLEAN
BOTH
BREADTH
BREAK
BROWSE
BULK
BY
CALL
CALLED
CARDINALITY
CASCADE
CASCADED
CASE
CAST
CATALOG
CHAR
CHAR_LENGTH
CHARACTER
CHARACTER_LENGTH
CHECK
CHECKPOINT
CLASS
CLOB
CLOSE
CLUSTERED
COALESCE
COLLATE
COLLATION
COLLECT
COLUMN
COMMIT
COMPLETION
COMPUTE
CONDITION
CONNECT
CONNECTION
CONSTRAINT
CONSTRAINTS
CONSTRUCTOR
CONTAINS
CONTAINSTABLE
CONTINUE
CONVERT
CORR
CORRESPONDING
COUNT
COVAR_POP
COVAR_SAMP
CREATE
CROSS
CUBE
CUME_DIST
CURRENT
CURRENT_CATALOG
CURRENT_DATE
CURRENT_DEFAULT_TRANSFORM_GROUP
CURRENT_PATH
CURRENT_ROLE
CURRENT_SCHEMA
CURRENT_TIME
CURRENT_TIMESTAMP
CURRENT_TRANSFORM_GROUP_FOR_TYPE
CURRENT_USER
CURSOR
CYCLE
DATA
DATABASE
DATE
DAY
DBCC
DEALLOCATE
DEC
DECIMAL
DECLARE
DEFAULT
DEFERRABLE
DEFERRED
DELETE
DENY
DEPTH
DEREF
DESC
DESCRIBE
DESCRIPTOR
DESTROY
DESTRUCTOR
DETERMINISTIC
DIAGNOSTICS
DICTIONARY
DISCONNECT
DISK
DISTINCT
DISTRIBUTED
DOMAIN
DOUBLE
DROP
DUMP
DYNAMIC
EACH
ELEMENT
ELSE
END
END-EXEC
EQUALS
ERRLVL
ESCAPE
EVERY
EXCEPT
EXCEPTION
EXEC
EXECUTE
EXISTS
EXIT
EXTERNAL
EXTRACT
FALSE
FETCH
FILE
FILLFACTOR
FILTER
FIRST
FLOAT
FOR
FOREIGN
FORTRAN
FOUND
FREE
FREETEXT
FREETEXTTABLE
FROM
FULL
FULLTEXTTABLE
FUNCTION
FUSION
GENERAL
GET
GLOBAL
GO
GOTO
GRANT
GROUP
GROUPING
HAVING
HOLD
HOLDLOCK
HOST
HOUR
IDENTITY
IDENTITY_INSERT
IDENTITYCOL
IF
IGNORE
IMMEDIATE
IN
INCLUDE
INDEX
INDICATOR
INITIALIZE
INITIALLY
INNER
INOUT
INPUT
INSENSITIVE
INSERT
INT
INTEGER
INTERSECT
INTERSECTION
INTERVAL
INTO
IS
ISOLATION
ITERATE
JOIN
KEY
KILL
LANGUAGE
LARGE
LAST
LATERAL
LEADING
LEFT
LESS
LEVEL
LIKE
LIKE_REGEX
LIMIT
LINENO
LN
LOAD
LOCAL
LOCALTIME
LOCALTIMESTAMP
LOCATOR
LOWER
MAP
MATCH
MAX
MEMBER
MERGE
METHOD
MIN
MINUTE
MOD
MODIFIES
MODIFY
MODULE
MONTH
MULTISET
NAMES
NATIONAL
NATURAL
NCHAR
NCLOB
NEW
NEXT
NO
NOCHECK
NONCLUSTERED
NONE
NORMALIZE
NOT
NULL
NULLIF
NUMERIC
OBJECT
OCCURRENCES_REGEX
OCTET_LENGTH
OF
OFF
OFFSETS
OLD
ON
ONLY
OPEN
OPENDATASOURCE
OPENQUERY
OPENROWSET
OPENXML
OPERATION
OPTION
OR
ORDER
ORDINALITY
OUT
OUTER
OUTPUT
OVER
OVERLAPS
OVERLAY
PAD
PARAMETER
PARAMETERS
PARTIAL
PARTITION
PASCAL
PATH
PERCENT
PERCENT_RANK
PERCENTILE_CONT
PERCENTILE_DISC
PIVOT
PLAN
POSITION
POSITION_REGEX
POSTFIX
PRECISION
PREFIX
PREORDER
PREPARE
PRESERVE
PRIMARY
PRINT
PRIOR
PRIVILEGES
PROC
PROCEDURE
PUBLIC
RAISERROR
RANGE
READ
READS
READTEXT
REAL
RECONFIGURE
RECURSIVE
REF
REFERENCES
REFERENCING
REGR_AVGX
REGR_AVGY
REGR_COUNT
REGR_INTERCEPT
REGR_R2
REGR_SLOPE
REGR_SXX
REGR_SXY
REGR_SYY
RELATIVE
RELEASE
REPLICATION
RESTORE
RESTRICT
RESULT
RETURN
RETURNS
REVERT
REVOKE
RIGHT
ROLE
ROLLBACK
ROLLUP
ROUTINE
ROW
ROWCOUNT
ROWGUIDCOL
ROWS
RULE
SAVE
SAVEPOINT
SCHEMA
SCOPE
SCROLL
SEARCH
SECOND
SECTION
SECURITYAUDIT
SELECT
SEMANTICKEYPHRASETABLE
SEMANTICSIMILARITYDETAILSTABLE
SEMANTICSIMILARITYTABLE
SENSITIVE
SEQUENCE
SESSION
SESSION_USER
SET
SETS
SETUSER
SHUTDOWN
SIMILAR
SIZE
SMALLINT
SOME
SPACE
SPECIFIC
SPECIFICTYPE
SQL
SQLCA
SQLCODE
SQLERROR
SQLEXCEPTION
SQLSTATE
SQLWARNING
START
STATE
STATEMENT
STATIC
STATISTICS
STDDEV_POP
STDDEV_SAMP
STRUCTURE
SUBMULTISET
SUBSTRING
SUBSTRING_REGEX
SUM
SYMMETRIC
SYSTEM
SYSTEM_USER
TABLE
TABLESAMPLE
TEMPORARY
TERMINATE
TEXTSIZE
THAN
THEN
TIME
TIMESTAMP
TIMEZONE_HOUR
TIMEZONE_MINUTE
TO
TOP
TRAILING
TRAN
TRANSACTION
TRANSLATE
TRANSLATE_REGEX
TRANSLATION
TREAT
TRIGGER
TRIM
TRUE
TRUNCATE
TRY_CONVERT
TSEQUAL
UESCAPE
UNDER
UNION
UNIQUE
UNKNOWN
UNNEST
UNPIVOT
UPDATE
UPDATETEXT
UPPER
USAGE
USE
USER
USING
VALUE
VALUES
VAR_POP
VAR_SAMP
VARCHAR
VARIABLE
VARYING
VIEW
WAITFOR
WHEN
WHENEVER
WHERE
WHILE
WIDTH_BUCKET
WINDOW
WITH
WITHIN
WITHOUT
WORK
WRITE
WRITETEXT
XMLAGG
XMLATTRIBUTES
XMLBINARY
XMLCAST
XMLCOMMENT
XMLCONCAT
XMLDOCUMENT
XMLELEMENT
XMLEXISTS
XMLFOREST
XMLITERATE
XMLNAMESPACES
XMLPARSE
XMLPI
XMLQUERY
XMLSERIALIZE
XMLTABLE
XMLTEXT
XMLVALIDATE
YEAR
ZONE";

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

            #region SQL Server data type names

@"
uniqueidentifier
bigint
int
tinyint
smallint
bit
cursor
hierarchyid
rowversion
xml
sql_variant
date
datetime
datetime2
datetimeoffset
smalldatetime
time
money
smallmoney
float
real
image
text
ntext
binary
varbinary
char
varchar
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
        [TestCase("tinyint", DbTypeFamily.Integer)]
        [TestCase("smallint", DbTypeFamily.Integer)]
        [TestCase("bit", DbTypeFamily.Boolean)]
        [TestCase("cursor", DbTypeFamily.Custom)]
        [TestCase("hierarchyid", DbTypeFamily.Custom)]
        [TestCase("rowversion", DbTypeFamily.Custom)]
        [TestCase("xml", DbTypeFamily.Custom)]
        [TestCase("sql_variant", DbTypeFamily.Custom)]
        [TestCase("date", DbTypeFamily.DateTime)]
        [TestCase("datetime", DbTypeFamily.DateTime)]
        [TestCase("datetime2", DbTypeFamily.DateTime)]
        [TestCase("datetimeoffset", DbTypeFamily.DateTime)]
        [TestCase("smalldatetime", DbTypeFamily.DateTime)]
        [TestCase("time", DbTypeFamily.Time)]
        [TestCase("money", DbTypeFamily.PreciseNumber)]
        [TestCase("smallmoney", DbTypeFamily.PreciseNumber)]
        [TestCase("float", DbTypeFamily.FloatingPointNumber)]
        [TestCase("real", DbTypeFamily.FloatingPointNumber)]
        [TestCase("image", DbTypeFamily.Binary)]
        [TestCase("text", DbTypeFamily.AnsiText)]
        [TestCase("ntext", DbTypeFamily.UnicodeText)]
        [TestCase("binary", DbTypeFamily.Binary)]
        [TestCase("varbinary", DbTypeFamily.Binary)]
        [TestCase("char", DbTypeFamily.AnsiText)]
        [TestCase("varchar", DbTypeFamily.AnsiText)]
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
        [TestCase("tinyint", DbTypeNameCategory.SingleWord)]
        [TestCase("smallint", DbTypeNameCategory.SingleWord)]
        [TestCase("bit", DbTypeNameCategory.SingleWord)]
        [TestCase("cursor", DbTypeNameCategory.SingleWord)]
        [TestCase("hierarchyid", DbTypeNameCategory.SingleWord)]
        [TestCase("rowversion", DbTypeNameCategory.SingleWord)]
        [TestCase("xml", DbTypeNameCategory.SingleWord)]
        [TestCase("sql_variant", DbTypeNameCategory.SingleWord)]
        [TestCase("date", DbTypeNameCategory.SingleWord)]
        [TestCase("datetime", DbTypeNameCategory.SingleWord)]
        [TestCase("datetime2", DbTypeNameCategory.SingleWord)]
        [TestCase("datetimeoffset", DbTypeNameCategory.SingleWord)]
        [TestCase("smalldatetime", DbTypeNameCategory.SingleWord)]
        [TestCase("time", DbTypeNameCategory.SingleWord)]
        [TestCase("money", DbTypeNameCategory.SingleWord)]
        [TestCase("smallmoney", DbTypeNameCategory.SingleWord)]
        [TestCase("float", DbTypeNameCategory.SingleWord)]
        [TestCase("real", DbTypeNameCategory.SingleWord)]
        [TestCase("image", DbTypeNameCategory.SingleWord)]
        [TestCase("text", DbTypeNameCategory.SingleWord)]
        [TestCase("ntext", DbTypeNameCategory.SingleWord)]
        [TestCase("binary", DbTypeNameCategory.Sized)]
        [TestCase("varbinary", DbTypeNameCategory.Sized)]
        [TestCase("char", DbTypeNameCategory.Sized)]
        [TestCase("varchar", DbTypeNameCategory.Sized)]
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

        [Test]
        [TestCase(
            "\t",
            "CHAR(9)",
            "CHAR(9)")]
        [TestCase(
            "hello\r\nbye\t ",
            "'hello' + CHAR(13) + CHAR(10) + 'bye' + CHAR(9) + ' '",
            "N'hello' + CHAR(13) + CHAR(10) + N'bye' + CHAR(9) + N' '")]
        [TestCase(
            "\t'Manya' and \r\n'Vanya'\n",
            "CHAR(9) + '''Manya'' and ' + CHAR(13) + CHAR(10) + '''Vanya''' + CHAR(10)",
            "CHAR(9) + N'''Manya'' and ' + CHAR(13) + CHAR(10) + N'''Vanya''' + CHAR(10)")]
        [TestCase(
            "'Almost' 'simple' 'string''",
            "'''Almost'' ''simple'' ''string'''''",
            "N'''Almost'' ''simple'' ''string'''''")]
        public void StringToSqlString_ValidArgument_ReturnsLiteral(string value, string expectedAnsiLiteral, string expectedUnicodeLiteral)
        {
            // Arrange
            
            // Act
            var ansiLiteral = _dialect.StringToSqlString(value, false);
            var unicodeLiteral = _dialect.StringToSqlString(value, true);

            // Assert
            Assert.That(ansiLiteral, Is.EqualTo(expectedAnsiLiteral));
            Assert.That(unicodeLiteral, Is.EqualTo(expectedUnicodeLiteral));
        }

    }
}
