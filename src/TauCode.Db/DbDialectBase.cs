using System.Data;
using TauCode.Db.Model;

namespace TauCode.Db;

public abstract class DbDialectBase : DbUtilityBase, IDbDialect
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
    private static readonly Dictionary<Type, DbDialectAttribute> _cache;

    static DbDialectBase()
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
        _cache = new Dictionary<Type, DbDialectAttribute>();
    }

    private static DbDialectAttribute GetOrCreateDialectAttribute(Type dialectType)
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
                    .GetCustomAttributes(typeof(DbDialectAttribute), false)
                    .Cast<DbDialectAttribute>()
                    .Single();

                _cache.Add(dialectType, attribute);

                return attribute;
            }
        }
    }

    #endregion

    #region Fields

    private string[] _reservedWords;
    private Tuple<char, char>[] _identifierDelimiters;

    #endregion

    #region Constructor

    protected DbDialectBase(string name)
        : base(null, false, true)
    {
        this.Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    #endregion

    #region Protected

    protected string[] BuildReservedWords()
    {
        var attribute = GetOrCreateDialectAttribute(this.GetType());
        return attribute.ReservedWords;
    }

    protected Tuple<char, char>[] BuildIdentifierDelimiters()
    {
        var attribute = DbDialectBase.GetOrCreateDialectAttribute(this.GetType());
        return attribute.IdentifierDelimiters;
    }

    #endregion

    #region IDialect Members

    public string Name { get; }

    public virtual IReadOnlyList<string> ReservedWords => _reservedWords ??= this.BuildReservedWords();

    public virtual string ClauseTerminator => ";";

    public virtual string UnicodeTextLiteralPrefix => string.Empty;

    public virtual bool IsClauseTerminatorMandatory => false;

    public virtual IReadOnlyList<Tuple<char, char>> IdentifierDelimiters => _identifierDelimiters ??= this.BuildIdentifierDelimiters();

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

        var closingDelimiterString = string.Empty;

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

    public virtual IList<IndexMold> GetCreatableIndexes(TableMold tableMold)
    {
        tableMold.CheckNotNullOrCorrupted(nameof(tableMold));
        return tableMold.Indexes.ToList();
    }

    #endregion
}