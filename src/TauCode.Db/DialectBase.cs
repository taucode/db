using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TauCode.Db.Exceptions;
using TauCode.Db.Model;

namespace TauCode.Db
{
    public abstract class DialectBase : IDialect
    {
        #region Constants

        protected const char SPACE = ' ';
        protected const char QUOTE = '\'';
        protected const char TAB = '\t';
        protected const char CR = '\r';
        protected const char LF = '\n';

        #endregion

        #region Static

        private static readonly List<char> _acceptableIdentifierFirstChars;
        private static readonly List<char> _acceptableIdentifierInnerChars;

        private static readonly object _cacheLock;
        private static readonly Dictionary<Type, DialectAttribute> _cache;

        static DialectBase()
        {
            // first identifier symbols
            _acceptableIdentifierFirstChars = new List<char>();
            for (var c = 'a'; c <= 'z'; c++)
            {
                _acceptableIdentifierFirstChars.Add(c);
            }

            for (var c = 'A'; c <= 'Z'; c++)
            {
                _acceptableIdentifierFirstChars.Add(c);
            }

            _acceptableIdentifierFirstChars.Add('_');

            // inner identifier symbols
            _acceptableIdentifierInnerChars = _acceptableIdentifierFirstChars.ToList();
            for (var c = '0'; c <= '9'; c++)
            {
                _acceptableIdentifierInnerChars.Add(c);
            }

            _cacheLock = new object();
            _cache = new Dictionary<Type, DialectAttribute>();
        }

        private static DialectAttribute GetOrCreateDialectAttribute(Type dialectType)
        {
            lock (_cacheLock)
            {
                if (_cache.ContainsKey(dialectType))
                {
                    return _cache[dialectType];
                }
                else
                {
                    var attribute = dialectType
                        .GetCustomAttributes(typeof(DialectAttribute), false)
                        .Cast<DialectAttribute>()
                        .Single();

                    _cache.Add(dialectType, attribute);

                    return attribute;
                }
            }
        }

        #endregion

        #region Fields

        private string[] _reservedWords;
        private string[] _dataTypeNames;
        private Tuple<char, char>[] _identifierDelimiters;

        #endregion

        #region Constructor

        protected DialectBase(string name)
        {
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        #endregion

        #region Protected

        protected string[] BuildDataTypeNames()
        {
            var attribute = GetOrCreateDialectAttribute(this.GetType());
            return attribute.DataTypeNames;
        }

        protected string[] BuildReservedWords()
        {
            var attribute = GetOrCreateDialectAttribute(this.GetType());
            return attribute.ReservedWords;
        }

        protected Tuple<char, char>[] BuildIdentifierDelimiters()
        {
            var attribute = DialectBase.GetOrCreateDialectAttribute(this.GetType());
            return attribute.IdentifierDelimiters;
        }

        protected ArgumentException CreateCouldNotConvertException(object obj, DbTypeFamily desiredFamily)
        {
            return new ArgumentException($"Could not convert value of type '{obj.GetType().FullName}' to {desiredFamily}");
        }

        protected virtual string CharFunctionName => "CHAR";

        #endregion

        #region IDialect Members

        public string Name { get; }

        public virtual IReadOnlyList<string> ReservedWords => _reservedWords ?? (_reservedWords = this.BuildReservedWords());

        public virtual IReadOnlyList<string> DataTypeNames => _dataTypeNames ?? (_dataTypeNames = this.BuildDataTypeNames());

        public virtual bool IsSingleWordTypeName(string typeName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            return
                this.DataTypeNames.Contains(typeName) &&
                this.GetTypeNameCategory(typeName) == DbTypeNameCategory.SingleWord;
        }

        public virtual bool IsSizedTypeName(string typeName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            return
                this.DataTypeNames.Contains(typeName) &&
                this.GetTypeNameCategory(typeName) == DbTypeNameCategory.Sized;
        }

        public virtual bool IsPreciseNumberTypeName(string typeName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            return
                this.DataTypeNames.Contains(typeName) &&
                this.GetTypeNameCategory(typeName) == DbTypeNameCategory.PreciseNumber;
        }

        public virtual string ClauseTerminator => ";";

        public virtual string UnicodeTextLiteralPrefix => string.Empty;

        public virtual bool IsClauseTerminatorMandatory => false;

        public virtual DbTypeFamily GetTypeFamily(string typeName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            typeName = typeName.ToLower();

            var attribute = DialectBase.GetOrCreateDialectAttribute(this.GetType());
            if (attribute.Families.ContainsKey(typeName))
            {
                return attribute.Families[typeName];
            }
            else
            {
                throw new UnknownDataTypeNameException(typeName: typeName);
            }
        }

        public virtual DbTypeNameCategory GetTypeNameCategory(string typeName)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            typeName = typeName.ToLower();

            var attribute = DialectBase.GetOrCreateDialectAttribute(this.GetType());
            if (attribute.Categories.ContainsKey(typeName))
            {
                return attribute.Categories[typeName];
            }
            else
            {
                throw new UnknownDataTypeNameException(typeName: typeName);
            }
        }

        public virtual IReadOnlyList<Tuple<char, char>> IdentifierDelimiters => _identifierDelimiters ?? (_identifierDelimiters = this.BuildIdentifierDelimiters());

        public virtual IReadOnlyList<char> AcceptableIdentifierFirstChars => _acceptableIdentifierFirstChars;

        public virtual IReadOnlyList<char> AcceptableIdentifierInnerChars => _acceptableIdentifierInnerChars;

        public virtual bool CanDecorateTypeIdentifier => true;

        public virtual string DecorateIdentifier(DbIdentifierType identifierType, string identifier, char? openingDelimiter)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            if (identifierType == DbIdentifierType.Type && !this.CanDecorateTypeIdentifier)
            {
                return identifier;
            }

            string closingDelimiterString = string.Empty;

            if (openingDelimiter.HasValue)
            {
                var tuple = this.IdentifierDelimiters.FirstOrDefault(x => x.Item1 == openingDelimiter.Value); // might be optimized, actually
                if (tuple == null)
                {
                    throw new ArgumentException($"Invalid opening delimiter: '{openingDelimiter.Value}.'", nameof(openingDelimiter));
                }

                closingDelimiterString = tuple.Item2.ToString();
            }

            return $"{openingDelimiter}{identifier}{closingDelimiterString}";
        }

        public virtual string ValueToSqlValueString(DbTypeMold type, object value)
        {
            if (value == null)
            {
                return "NULL";
            }

            var family = this.GetTypeFamily(type.Name);
            string literal;

            switch (family)
            {
                case DbTypeFamily.Uniqueidentifier:
                    if (value is Guid guid)
                    {
                        literal = $"'{guid:D}'";
                    }
                    else if (value is string guidString)
                    {
                        var str = (new Guid(guidString)).ToString("D");
                        literal = this.StringToSqlString(str, false);
                    }
                    else
                    {
                        throw this.CreateCouldNotConvertException(value, family);
                    }
                    break;

                case DbTypeFamily.AnsiText:
                    if (value is string ansiText)
                    {
                        literal = this.StringToSqlString(ansiText, false);
                    }
                    else
                    {
                        throw this.CreateCouldNotConvertException(value, family);
                    }
                    break;

                case DbTypeFamily.UnicodeText:
                    if (value is string unicodeText)
                    {
                        literal = this.StringToSqlString(unicodeText, true);
                    }
                    else
                    {
                        throw this.CreateCouldNotConvertException(value, family);
                    }
                    break;

                case DbTypeFamily.Binary:
                    if (value is byte[] byteArray)
                    {
                        literal = UtilsHelper.ByteArrayToHex(byteArray);
                    }
                    else
                    {
                        throw this.CreateCouldNotConvertException(value, family);
                    }
                    break;

                case DbTypeFamily.DateTime:
                    if (value is DateTime dateTime)
                    {
                        literal = $"'{dateTime:yyyy-MM-ddTHH:mm:ss.fffZ}'";
                    }
                    else if (value is string dateTimeString)
                    {
                        var parsed = DateTime.TryParse(dateTimeString, out var dt);
                        if (parsed)
                        {
                            literal = $"'{dt:yyyy-MM-ddTHH:mm:ss.fffZ}'";
                        }
                        else
                        {
                            throw this.CreateCouldNotConvertException(value, family);
                        }
                    }
                    else
                    {
                        throw this.CreateCouldNotConvertException(value, family);
                    }
                    break;

                case DbTypeFamily.Time:
                    throw new ArgumentException($"'DbTypeFamily.Time' is not currently supported.", nameof(value));

                case DbTypeFamily.Boolean:
                    if (value is bool boolValue)
                    {
                        literal = boolValue ? "1" : "0";
                    }
                    else if (value is IConvertible convertible2)
                    {
                        var intValue = convertible2.ToInt32(CultureInfo.InvariantCulture);
                        literal = intValue != 0 ? "1" : "0";
                    }
                    else
                    {
                        throw this.CreateCouldNotConvertException(value, family);
                    }
                    break;

                case DbTypeFamily.Integer:
                    if (value is IConvertible convertible)
                    {
                        var longValue = convertible.ToInt64(CultureInfo.InvariantCulture);
                        literal = longValue.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        throw this.CreateCouldNotConvertException(value, family);
                    }
                    break;

                case DbTypeFamily.PreciseNumber:
                    if (value is IConvertible convertible3)
                    {
                        var decimalValue = convertible3.ToDecimal(CultureInfo.InvariantCulture);
                        literal = decimalValue.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        throw this.CreateCouldNotConvertException(value, family);
                    }
                    break;

                case DbTypeFamily.FloatingPointNumber:
                    if (value is double doubleValue)
                    {
                        literal = doubleValue.ToString(CultureInfo.InvariantCulture);
                    }
                    else if (value is float floatValue)
                    {
                        literal = floatValue.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        throw this.CreateCouldNotConvertException(value, family);
                    }
                    break;

                default:
                    throw new ArgumentException($"Value cannot be converted.", nameof(value));
            }

            return literal;
        }

        public virtual DbTypeMold ResolveType(string typeName, int? size, int? precision, int? scale)
        {
            if (typeName == null)
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            var dbTypeMold = new DbTypeMold();

            var nameCategory = this.GetTypeNameCategory(typeName);

            dbTypeMold.Name = typeName;

            switch (nameCategory)
            {
                case DbTypeNameCategory.SingleWord:
                    // no action
                    break;

                case DbTypeNameCategory.Sized:
                    dbTypeMold.Size = size;
                    break;

                case DbTypeNameCategory.PreciseNumber:
                    dbTypeMold.Precision = precision;
                    dbTypeMold.Scale = scale;
                    break;

                default:
                    throw new ArgumentException($"Could not resolve type '{typeName}'", nameof(typeName));
            }

            return dbTypeMold;
        }

        public virtual string StringToSqlString(string value, bool isUnicode)
        {
            if (value == null)
            {
                return "NULL";
            }

            var normalStringOpened = false;
            var sb = new StringBuilder();

            if (value.Length == 0)
            {
                if (isUnicode)
                {
                    sb.Append(this.UnicodeTextLiteralPrefix);
                }

                sb.Append("''");
            }

            for (var i = 0; i < value.Length; i++)
            {
                var c = value[i];
                if (c < SPACE)
                {
                    if (c == TAB || c == CR || c == LF)
                    {
                        if (normalStringOpened)
                        {
                            sb.Append("'"); // close normal string
                        }

                        normalStringOpened = false;

                        if (i == 0)
                        {
                            // don't add anything
                        }
                        else
                        {
                            sb.Append(" + ");
                        }

                        sb.Append($"{this.CharFunctionName}({(int)c})");
                    }
                    else
                    {
                        throw new ArgumentException($"Value contains unsupported char ({(int)c}).", nameof(value));
                    }
                }
                else
                {
                    if (normalStringOpened)
                    {
                        // keep working
                    }
                    else
                    {
                        if (i == 0)
                        {
                            // don't add anything
                        }
                        else
                        {
                            sb.Append(" + ");
                        }

                        // open string
                        if (isUnicode)
                        {
                            sb.Append(this.UnicodeTextLiteralPrefix);
                        }

                        sb.Append("'");
                        normalStringOpened = true;
                    }

                    sb.Append(c == QUOTE ? "''" : c.ToString());
                }
            }

            if (normalStringOpened)
            {
                // need to close normal string
                sb.Append("'");
            }

            return sb.ToString();
        }

        #endregion
    }
}
