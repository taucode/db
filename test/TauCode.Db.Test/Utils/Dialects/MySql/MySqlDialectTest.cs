using NUnit.Framework;
using System;
using System.Linq;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;
using TauCode.Db.Utils.Dialects;
using TauCode.Db.Utils.Dialects.MySql;

namespace TauCode.Db.Test.Utils.Dialects.MySql
{
    [TestFixture]
    public class MySqlDialectTest
    {
        private IDialect _dialect;

        [SetUp]
        public void SetUp()
        {
            _dialect = MySqlDialect.Instance;
        }

        [Test]
        public void Name_NoArguments_ReturnsValidName()
        {
            // Arrange

            // Act
            
            // Assert
            Assert.That(_dialect.Name, Is.EqualTo("MySQL"));
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

            #region MySQL reserved words

@"
ACCESSIBLE
ACCOUNT
ACTION
ACTIVE
ADD
ADMIN
AFTER
AGAINST
AGGREGATE
ALGORITHM
ALL
ALTER
ALWAYS
ANALYSE
ANALYZE
AND
ANY
AS
ASC
ASCII
ASENSITIVE
AT
AUTO_INCREMENT
AUTOEXTEND_SIZE
AVG
AVG_ROW_LENGTH
BACKUP
BEFORE
BEGIN
BETWEEN
BIGINT
BINARY
BINLOG
BIT
BLOB
BLOCK
BOOL
BOOLEAN
BOTH
BTREE
BUCKETS
BY
BYTE
CACHE
CALL
CASCADE
CASCADED
CASE
CATALOG_NAME
CHAIN
CHANGE
CHANGED
CHANNEL
CHAR
CHARACTER
CHARSET
CHECK
CHECKSUM
CIPHER
CLASS_ORIGIN
CLIENT
CLONE
CLOSE
COALESCE
CODE
COLLATE
COLLATION
COLUMN
COLUMN_FORMAT
COLUMN_NAME
COLUMNS
COMMENT
COMMIT
COMMITTED
COMPACT
COMPLETION
COMPONENT
COMPRESSED
COMPRESSION
CONCURRENT
CONDITION
CONNECTION
CONSISTENT
CONSTRAINT
CONSTRAINT_CATALOG
CONSTRAINT_NAME
CONSTRAINT_SCHEMA
CONTAINS
CONTEXT
CONTINUE
CONVERT
CPU
CREATE
CROSS
CUBE
CUME_DIST
CURRENT
CURRENT_DATE
CURRENT_TIME
CURRENT_TIMESTAMP
CURRENT_USER
CURSOR
CURSOR_NAME
DATA
DATABASE
DATABASES
DATAFILE
DATE
DATETIME
DAY
DAY_HOUR
DAY_MICROSECOND
DAY_MINUTE
DAY_SECOND
DEALLOCATE
DEC
DECIMAL
DECLARE
DEFAULT
DEFAULT_AUTH
DEFINER
DEFINITION
DELAY_KEY_WRITE
DELAYED
DELETE
DENSE_RANK
DES_KEY_FILE
DESC
DESCRIBE
DESCRIPTION
DETERMINISTIC
DIAGNOSTICS
DIRECTORY
DISABLE
DISCARD
DISK
DISTINCT
DISTINCTROW
DIV
DO
DOUBLE
DROP
DUAL
DUMPFILE
DUPLICATE
DYNAMIC
EACH
ELSE
ELSEIF
EMPTY
ENABLE
ENCLOSED
ENCRYPTION
END
ENDS
ENGINE
ENGINES
ENUM
ERROR
ERRORS
ESCAPE
ESCAPED
EVENT
EVENTS
EVERY
EXCEPT
EXCHANGE
EXCLUDE
EXECUTE
EXISTS
EXIT
EXPANSION
EXPIRE
EXPLAIN
EXPORT
EXTENDED
EXTENT_SIZE
FALSE
FAST
FAULTS
FETCH
FIELDS
FILE
FILE_BLOCK_SIZE
FILTER
FIRST
FIRST_VALUE
FIXED
FLOAT
FLOAT4
FLOAT8
FLUSH
FOLLOWING
FOLLOWS
FOR
FORCE
FOREIGN
FORMAT
FOUND
FROM
FULL
FULLTEXT
FUNCTION
GENERAL
GENERATED
GEOMCOLLECTION
GEOMETRY
GEOMETRYCOLLECTION
GET
GET_FORMAT
GET_MASTER_PUBLIC_KEY
GLOBAL
GRANT
GRANTS
GROUP
GROUP_REPLICATION
GROUPING
GROUPS
HANDLER
HASH
HAVING
HELP
HIGH_PRIORITY
HISTOGRAM
HISTORY
HOST
HOSTS
HOUR
HOUR_MICROSECOND
HOUR_MINUTE
HOUR_SECOND
IDENTIFIED
IF
IGNORE
IGNORE_SERVER_IDS
IMPORT
IN
INACTIVE
INDEX
INDEXES
INFILE
INITIAL_SIZE
INNER
INOUT
INSENSITIVE
INSERT
INSERT_METHOD
INSTALL
INSTANCE
INT
INT1
INT2
INT3
INT4
INT8
INTEGER
INTERVAL
INTO
INVISIBLE
INVOKER
IO
IO_AFTER_GTIDS
IO_BEFORE_GTIDS
IO_THREAD
IPC
IS
ISOLATION
ISSUER
ITERATE
JOIN
JSON
JSON_TABLE
KEY
KEY_BLOCK_SIZE
KEYS
KILL
LAG
LANGUAGE
LAST
LAST_VALUE
LATERAL
LEAD
LEADING
LEAVE
LEAVES
LEFT
LESS
LEVEL
LIKE
LIMIT
LINEAR
LINES
LINESTRING
LIST
LOAD
LOCAL
LOCALTIME
LOCALTIMESTAMP
LOCK
LOCKED
LOCKS
LOGFILE
LOGS
LONG
LONGBLOB
LONGTEXT
LOOP
LOW_PRIORITY
MASTER
MASTER_AUTO_POSITION
MASTER_BIND
MASTER_CONNECT_RETRY
MASTER_DELAY
MASTER_HEARTBEAT_PERIOD
MASTER_HOST
MASTER_LOG_FILE
MASTER_LOG_POS
MASTER_PASSWORD
MASTER_PORT
MASTER_PUBLIC_KEY_PATH
MASTER_RETRY_COUNT
MASTER_SERVER_ID
MASTER_SSL
MASTER_SSL_CA
MASTER_SSL_CAPATH
MASTER_SSL_CERT
MASTER_SSL_CIPHER
MASTER_SSL_CRL
MASTER_SSL_CRLPATH
MASTER_SSL_KEY
MASTER_SSL_VERIFY_SERVER_CERT
MASTER_TLS_VERSION
MASTER_USER
MATCH
MAX_CONNECTIONS_PER_HOUR
MAX_QUERIES_PER_HOUR
MAX_ROWS
MAX_SIZE
MAX_UPDATES_PER_HOUR
MAX_USER_CONNECTIONS
MAXVALUE
MEDIUM
MEDIUMBLOB
MEDIUMINT
MEDIUMTEXT
MEMORY
MERGE
MESSAGE_TEXT
MICROSECOND
MIDDLEINT
MIGRATE
MIN_ROWS
MINUTE
MINUTE_MICROSECOND
MINUTE_SECOND
MOD
MODE
MODIFIES
MODIFY
MONTH
MULTILINESTRING
MULTIPOINT
MULTIPOLYGON
MUTEX
MYSQL_ERRNO
NAME
NAMES
NATIONAL
NATURAL
NCHAR
NDB
NDBCLUSTER
NESTED
NEVER
NEW
NEXT
NO
NO_WAIT
NO_WRITE_TO_BINLOG
NODEGROUP
NONE
NOT
NOWAIT
NTH_VALUE
NTILE
NULL
NULLS
NUMBER
NUMERIC
NVARCHAR
OF
OFFSET
OLD
ON
ONE
ONLY
OPEN
OPTIMIZE
OPTIMIZER_COSTS
OPTION
OPTIONAL
OPTIONALLY
OPTIONS
OR
ORDER
ORDINALITY
ORGANIZATION
OTHERS
OUT
OUTER
OUTFILE
OVER
OWNER
PACK_KEYS
PAGE
PARSE_GCOL_EXPR
PARSER
PARTIAL
PARTITION
PARTITIONING
PARTITIONS
PASSWORD
PATH
PERCENT_RANK
PERSIST
PERSIST_ONLY
PHASE
PLUGIN
PLUGIN_DIR
PLUGINS
POINT
POLYGON
PORT
PRECEDES
PRECEDING
PRECISION
PREPARE
PRESERVE
PREV
PRIMARY
PRIVILEGES
PROCEDURE
PROCESS
PROCESSLIST
PROFILE
PROFILES
PROXY
PURGE
QUARTER
QUERY
QUICK
RANGE
RANK
READ
READ_ONLY
READ_WRITE
READS
REAL
REBUILD
RECOVER
RECURSIVE
REDO_BUFFER_SIZE
REDOFILE
REDUNDANT
REFERENCE
REFERENCES
REGEXP
RELAY
RELAY_LOG_FILE
RELAY_LOG_POS
RELAY_THREAD
RELAYLOG
RELEASE
RELOAD
REMOTE
REMOVE
RENAME
REORGANIZE
REPAIR
REPEAT
REPEATABLE
REPLACE
REPLICATE_DO_DB
REPLICATE_DO_TABLE
REPLICATE_IGNORE_DB
REPLICATE_IGNORE_TABLE
REPLICATE_REWRITE_DB
REPLICATE_WILD_DO_TABLE
REPLICATE_WILD_IGNORE_TABLE
REPLICATION
REQUIRE
RESET
RESIGNAL
RESOURCE
RESPECT
RESTART
RESTORE
RESTRICT
RESUME
RETAIN
RETURN
RETURNED_SQLSTATE
RETURNS
REUSE
REVERSE
REVOKE
RIGHT
RLIKE
ROLE
ROLLBACK
ROLLUP
ROTATE
ROUTINE
ROW
ROW_COUNT
ROW_FORMAT
ROW_NUMBER
ROWS
RTREE
SAVEPOINT
SCHEDULE
SCHEMA
SCHEMA_NAME
SCHEMAS
SECOND
SECOND_MICROSECOND
SECONDARY_ENGINE
SECONDARY_LOAD
SECONDARY_UNLOAD
SECURITY
SELECT
SENSITIVE
SEPARATOR
SERIAL
SERIALIZABLE
SERVER
SESSION
SET
SHARE
SHOW
SHUTDOWN
SIGNAL
SIGNED
SIMPLE
SKIP
SLAVE
SLOW
SMALLINT
SNAPSHOT
SOCKET
SOME
SONAME
SOUNDS
SOURCE
SPATIAL
SPECIFIC
SQL
SQL_AFTER_GTIDS
SQL_AFTER_MTS_GAPS
SQL_BEFORE_GTIDS
SQL_BIG_RESULT
SQL_BUFFER_RESULT
SQL_CACHE
SQL_CALC_FOUND_ROWS
SQL_NO_CACHE
SQL_SMALL_RESULT
SQL_THREAD
SQL_TSI_DAY
SQL_TSI_HOUR
SQL_TSI_MINUTE
SQL_TSI_MONTH
SQL_TSI_QUARTER
SQL_TSI_SECOND
SQL_TSI_WEEK
SQL_TSI_YEAR
SQLEXCEPTION
SQLSTATE
SQLWARNING
SRID
SSL
STACKED
START
STARTING
STARTS
STATS_AUTO_RECALC
STATS_PERSISTENT
STATS_SAMPLE_PAGES
STATUS
STOP
STORAGE
STORED
STRAIGHT_JOIN
STRING
SUBCLASS_ORIGIN
SUBJECT
SUBPARTITION
SUBPARTITIONS
SUPER
SUSPEND
SWAPS
SWITCHES
SYSTEM
TABLE
TABLE_CHECKSUM
TABLE_NAME
TABLES
TABLESPACE
TEMPORARY
TEMPTABLE
TERMINATED
TEXT
THAN
THEN
THREAD_PRIORITY
TIES
TIME
TIMESTAMP
TIMESTAMPADD
TIMESTAMPDIFF
TINYBLOB
TINYINT
TINYTEXT
TO
TRAILING
TRANSACTION
TRIGGER
TRIGGERS
TRUE
TRUNCATE
TYPE
TYPES
UNBOUNDED
UNCOMMITTED
UNDEFINED
UNDO
UNDO_BUFFER_SIZE
UNDOFILE
UNICODE
UNINSTALL
UNION
UNIQUE
UNKNOWN
UNLOCK
UNSIGNED
UNTIL
UPDATE
UPGRADE
USAGE
USE
USE_FRM
USER
USER_RESOURCES
USING
UTC_DATE
UTC_TIME
UTC_TIMESTAMP
VALIDATION
VALUE
VALUES
VARBINARY
VARCHAR
VARCHARACTER
VARIABLES
VARYING
VCPU
VIEW
VIRTUAL
VISIBLE
WAIT
WARNINGS
WEEK
WEIGHT_STRING
WHEN
WHERE
WHILE
WINDOW
WITH
WITHOUT
WORK
WRAPPER
WRITE
X509
XA
XID
XML
XOR
YEAR
YEAR_MONTH
ZEROFILL";

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
            Assert.That(tuple.Item1, Is.EqualTo('`'));
            Assert.That(tuple.Item2, Is.EqualTo('`'));

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
            var decoratedIndex = _dialect.DecorateIdentifier(DbIdentifierType.Index, "the_index", '`');

            // Assert
            Assert.That(decoratedTable, Is.EqualTo("\"the_table\""));
            Assert.That(decoratedIndex, Is.EqualTo("`the_index`"));
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
        public void DecoratableTypeIdentifier_NoArguments_ReturnsFalse()
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

            #region MySQL data type names

@"
bit
int
integer
smallint
bigint
tinyint
year
blob
longblob
mediumblob
tinyblob
double
float
date
datetime
timestamp
time
json
text
longtext
mediumint
mediumtext
tinytext
char
nchar
nvarchar
varchar
binary
varbinary
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
        [TestCase("bit", DbTypeFamily.Integer)]
        [TestCase("int", DbTypeFamily.Integer)]
        [TestCase("integer", DbTypeFamily.Integer)]
        [TestCase("smallint", DbTypeFamily.Integer)]
        [TestCase("bigint", DbTypeFamily.Integer)]
        [TestCase("tinyint", DbTypeFamily.Integer)]
        [TestCase("year", DbTypeFamily.Integer)]
        [TestCase("blob", DbTypeFamily.Binary)]
        [TestCase("longblob", DbTypeFamily.Binary)]
        [TestCase("mediumblob", DbTypeFamily.Binary)]
        [TestCase("tinyblob", DbTypeFamily.Binary)]
        [TestCase("double", DbTypeFamily.FloatingPointNumber)]
        [TestCase("float", DbTypeFamily.FloatingPointNumber)]
        [TestCase("date", DbTypeFamily.DateTime)]
        [TestCase("datetime", DbTypeFamily.DateTime)]
        [TestCase("timestamp", DbTypeFamily.DateTime)]
        [TestCase("time", DbTypeFamily.Time)]
        [TestCase("json", DbTypeFamily.Custom)]
        [TestCase("text", DbTypeFamily.UnicodeText)]
        [TestCase("longtext", DbTypeFamily.UnicodeText)]
        [TestCase("mediumint", DbTypeFamily.Integer)]
        [TestCase("mediumtext", DbTypeFamily.UnicodeText)]
        [TestCase("tinytext", DbTypeFamily.UnicodeText)]
        [TestCase("char", DbTypeFamily.UnicodeText)]
        [TestCase("nchar", DbTypeFamily.UnicodeText)]
        [TestCase("nvarchar", DbTypeFamily.UnicodeText)]
        [TestCase("varchar", DbTypeFamily.UnicodeText)]
        [TestCase("binary", DbTypeFamily.Binary)]
        [TestCase("varbinary", DbTypeFamily.Binary)]
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
        [TestCase("bit", DbTypeNameCategory.SingleWord)]
        [TestCase("int", DbTypeNameCategory.SingleWord)]
        [TestCase("integer", DbTypeNameCategory.SingleWord)]
        [TestCase("smallint", DbTypeNameCategory.SingleWord)]
        [TestCase("bigint", DbTypeNameCategory.SingleWord)]
        [TestCase("tinyint", DbTypeNameCategory.SingleWord)]
        [TestCase("year", DbTypeNameCategory.SingleWord)]
        [TestCase("mediumint", DbTypeNameCategory.SingleWord)]
        [TestCase("blob", DbTypeNameCategory.SingleWord)]
        [TestCase("longblob", DbTypeNameCategory.SingleWord)]
        [TestCase("mediumblob", DbTypeNameCategory.SingleWord)]
        [TestCase("tinyblob", DbTypeNameCategory.SingleWord)]
        [TestCase("double", DbTypeNameCategory.SingleWord)]
        [TestCase("float", DbTypeNameCategory.SingleWord)]
        [TestCase("date", DbTypeNameCategory.SingleWord)]
        [TestCase("datetime", DbTypeNameCategory.SingleWord)]
        [TestCase("timestamp", DbTypeNameCategory.SingleWord)]
        [TestCase("time", DbTypeNameCategory.SingleWord)]
        [TestCase("json", DbTypeNameCategory.SingleWord)]
        [TestCase("text", DbTypeNameCategory.SingleWord)]
        [TestCase("longtext", DbTypeNameCategory.SingleWord)]
        [TestCase("mediumtext", DbTypeNameCategory.SingleWord)]
        [TestCase("tinytext", DbTypeNameCategory.SingleWord)]
        [TestCase("char", DbTypeNameCategory.Sized)]
        [TestCase("nchar", DbTypeNameCategory.Sized)]
        [TestCase("nvarchar", DbTypeNameCategory.Sized)]
        [TestCase("varchar", DbTypeNameCategory.Sized)]
        [TestCase("binary", DbTypeNameCategory.Sized)]
        [TestCase("varbinary", DbTypeNameCategory.Sized)]
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
            "'hello' + CHAR(13) + CHAR(10) + 'bye' + CHAR(9) + ' '")]
        [TestCase(
            "\t'Manya' and \r\n'Vanya'\n",
            "CHAR(9) + '''Manya'' and ' + CHAR(13) + CHAR(10) + '''Vanya''' + CHAR(10)",
            "CHAR(9) + '''Manya'' and ' + CHAR(13) + CHAR(10) + '''Vanya''' + CHAR(10)")]
        [TestCase(
            "'Almost' 'simple' 'string''",
            "'''Almost'' ''simple'' ''string'''''",
            "'''Almost'' ''simple'' ''string'''''")]
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
