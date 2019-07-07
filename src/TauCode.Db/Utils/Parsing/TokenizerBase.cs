using System;
using System.Collections.Generic;
using System.Linq;
using TauCode.Db.Exceptions;
using TauCode.Db.Utils.Dialects;
using TauCode.Db.Utils.Parsing.Core;
using TauCode.Db.Utils.Parsing.Core.Tokens;

namespace TauCode.Db.Utils.Parsing
{
    public abstract class TokenizerBase : ITokenizer
    {
        #region Constants

        private const char TAB = '\t';
        private const char CR = '\r';
        private const char LF = '\n';

        #endregion

        #region Fields

        private TokenCollection _tokens;
        private string _input;
        private int _position;

        private readonly HashSet<char> _acceptableIdentifierFirstChars;
        private readonly HashSet<char> _acceptableIdentifierInnerChars;
        private readonly HashSet<char> _symbolChars;
        private readonly HashSet<char> _digitChars;
        private readonly Dictionary<char, char> _identifierDelimiters;

        #endregion

        #region Constructor

        protected TokenizerBase(IDialect dialect)
        {
            this.Dialect = dialect ?? throw new ArgumentNullException(nameof(dialect));

            _acceptableIdentifierFirstChars = new HashSet<char>(this.Dialect.AcceptableIdentifierFirstChars);
            _acceptableIdentifierInnerChars = new HashSet<char>(this.Dialect.AcceptableIdentifierInnerChars);

            _identifierDelimiters = this.Dialect.IdentifierDelimiters
                .ToDictionary(x => x.Item1, x => x.Item2);

            // symbol chars
            _symbolChars = new HashSet<char>(new[] { '(', ')', ',', ';' });

            // digit chars
            _digitChars = new HashSet<char>(Enumerable.Range('0', 10).Select(x => (char)x));
        }

        #endregion

        #region Polymorph

        protected char GetCurrentChar()
        {
            return _input[_position];
        }

        protected void Advance(int delta = 1)
        {
            _position += delta;
        }

        protected bool IsEndOfInput()
        {
            return _position == _input.Length;
        }

        protected int GetCurrentPosition() => _position;

        protected virtual bool IsAcceptableIdentifierFirstChar(char c)
        {
            return _acceptableIdentifierFirstChars.Contains(c);
        }

        protected virtual bool IsAcceptableIdentifierInnerChar(char c)
        {
            return _acceptableIdentifierInnerChars.Contains(c);
        }

        protected virtual bool IsSpaceChar(char c)
        {
            return char.IsWhiteSpace(c);
        }

        protected virtual bool IsSymbolChar(char c)
        {
            return _symbolChars.Contains(c);
        }

        protected virtual bool IsDigitSymbol(char c)
        {
            return _digitChars.Contains(c);
        }

        protected virtual bool IsIdentifierOpeningDelimiter(char c)
        {
            return _identifierDelimiters.ContainsKey(c);
        }

        protected virtual bool IsStringOpeningDelimiter(char c)
        {
            return c == '\'';
        }

        protected virtual WordToken ExtractWordToken()
        {
            var begin = this.GetCurrentPosition();

            while (true)
            {
                if (this.IsEndOfInput())
                {
                    throw new TokenizerException("Expected word token, but end of input encountered.");
                }

                var c = GetCurrentChar();

                if (this.IsSpaceChar(c) || this.IsSymbolChar(c))
                {
                    break;
                }
                else if (this.IsAcceptableIdentifierInnerChar(c))
                {
                    this.Advance();
                }
                else
                {
                    throw new TokenizerException($"Expected word token, but character '{c}' encountered.");
                }
            }

            var end = this.GetCurrentPosition();
            if (begin == end)
            {
                throw new TokenizerException("Failed to extract word token.");
            }

            var word = _input.Substring(begin, end - begin);
            var wordToken = new WordToken(word);
            return wordToken;
        }

        protected virtual IdentifierToken ExtractIdentifierToken(char closingDelimiter)
        {
            this.Advance(); // skip opening delimiter

            var begin = this.GetCurrentPosition(); // position of delimiter
            var consumedAtLeastOneIdentifierChar = false;

            while (true)
            {
                if (this.IsEndOfInput())
                {
                    throw new TokenizerException("Expected identifier, but end of input encountered.");
                }

                var c = GetCurrentChar();

                if (this.IsSpaceChar(c) || this.IsSymbolChar(c))
                {
                    throw new TokenizerException("Haven't encountered a closing delimiter.");
                }
                else if (this.IsAcceptableIdentifierInnerChar(c))
                {
                    // can be inner char of identifier

                    if (this.IsAcceptableIdentifierFirstChar(c))
                    {
                        // can be first char of identifier

                        this.Advance();
                        consumedAtLeastOneIdentifierChar = true;
                    }
                    else
                    {
                        // can be inner, cannot be first (e.g. '1')
                        if (consumedAtLeastOneIdentifierChar)
                        {
                            // ok
                            this.Advance();
                        }
                        else
                        {
                            throw new TokenizerException($"Expected identifier, but character '{c}' encountered.");
                        }
                    }
                }
                else if (c == closingDelimiter)
                {
                    break;
                }
                else
                {
                    throw new TokenizerException("Failed to extract identifier.");
                }
            }

            var end = this.GetCurrentPosition();
            this.Advance(); // skip closing delimiter

            if (begin == end)
            {
                throw new TokenizerException("Failed to extract identifier.");
            }

            var identifier = _input.Substring(begin, end - begin);
            var identifierToken = new IdentifierToken(identifier);
            return identifierToken;
        }

        protected virtual SymbolToken ExtractSymbolToken()
        {
            var c = this.GetCurrentChar();
            SymbolToken symbolToken;

            switch (c)
            {
                case '(':
                    symbolToken = SymbolToken.OpeningRoundBracketSymbol;
                    this.Advance();
                    break;

                case ')':
                    symbolToken = SymbolToken.ClosingRoundBracketSymbol;
                    this.Advance();
                    break;

                case ',':
                    symbolToken = SymbolToken.CommaSymbol;
                    this.Advance();
                    break;

                case ';':
                    symbolToken = SymbolToken.SemicolonSymbol;
                    this.Advance();
                    break;

                default:
                    throw new TokenizerException($"Expected symbol, but character '{c}' encountered.");
            }

            return symbolToken;
        }

        protected virtual NumberToken ExtractNumberToken()
        {
            var begin = this.GetCurrentPosition();

            while (true)
            {
                if (this.IsEndOfInput())
                {
                    break; // no problem, end of number and end of input
                }

                var c = this.GetCurrentChar();

                if (this.IsDigitSymbol(c))
                {
                    this.Advance();
                }
                else if (this.IsSpaceChar(c))
                {
                    break;
                }
                else if (this.IsSymbolChar(c))
                {
                    break;
                }
                else
                {
                    throw new TokenizerException($"Expected number, but character '{c}' encountered.");
                }
            }

            var end = this.GetCurrentPosition();
            if (begin == end)
            {
                throw new TokenizerException("Failed to extract number.");
            }

            var number = _input.Substring(begin, end - begin);
            var numberToken = new NumberToken(number);
            return numberToken;
        }

        protected virtual StringToken ExtractStringToken()
        {
            this.Advance(); // skip opening delimiter

            var begin = this.GetCurrentPosition();

            while (true)
            {
                if (this.IsEndOfInput())
                {
                    throw new TokenizerException("Expected string, but end of input encountered.");
                }

                var c = this.GetCurrentChar();

                if (c == CR || c == LF)
                {
                    throw new TokenizerException("Newline in string.");
                }
                else if (c == '\'')
                {
                    break;
                }

                this.Advance();
            }

            var end = this.GetCurrentPosition();
            var str = _input.Substring(begin, end - begin);
            var token = new StringToken(str);

            this.Advance(); // skip closing '

            return token;
        }

        #endregion

        #region ITokenizer Members

        public IDialect Dialect { get; }

        public int? TabSize { get; set; }

        public virtual Token[] Tokenize(string input)
        {
            _input = input ?? throw new ArgumentNullException(nameof(input));
            _tokens = new TokenCollection();
            _position = 0;

            while (true)
            {
                if (this.IsEndOfInput())
                {
                    break;
                }

                var c = this.GetCurrentChar();
                if (this.IsAcceptableIdentifierFirstChar(c))
                {
                    var token = this.ExtractWordToken();
                    
                    _tokens.Add(token, default(TokenCollection.TokenPosition));
                }
                else if (this.IsSpaceChar(c))
                {
                    if (c == TAB)
                    {
                        // deal later...
                    }
                    else if (c == CR)
                    {
                        // deal later...
                    }
                    else if (c == LF)
                    {
                        // deal later...
                    }

                    this.Advance();
                }
                else if (this.IsSymbolChar(c))
                {
                    var token = this.ExtractSymbolToken();
                    _tokens.Add(token, default);
                }
                else if (this.IsDigitSymbol(c))
                {
                    var token = this.ExtractNumberToken();
                    _tokens.Add(token, default);
                }
                else if (this.IsIdentifierOpeningDelimiter(c))
                {
                    var token = this.ExtractIdentifierToken(_identifierDelimiters[c]);
                    _tokens.Add(token, default);
                }
                else if (this.IsStringOpeningDelimiter(c))
                {
                    var token = this.ExtractStringToken();
                    _tokens.Add(token, default);
                }
                else
                {
                    throw new TokenizerException($"Unexpected character '{c}' encountered.");
                }
            }

            return _tokens.ToArray();
        }

        #endregion
    }
}
