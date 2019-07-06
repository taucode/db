using System;
using System.Collections;
using System.Collections.Generic;
using TauCode.Db.Utils.Parsing.Core;

namespace TauCode.Db.Utils.Parsing
{
    public class TokenCollection : IReadOnlyList<Token>
    {
        #region Nested

        public struct TokenPosition
        {
            public readonly int TokenIndex;
            public readonly int AbsoluteCharIndex;
            public readonly Position VisiblePosition;
            public readonly int LineCharIndex;

            public TokenPosition(
                int tokenIndex,
                int absoluteCharIndex,
                Position visiblePosition,
                int lineCharIndex)
            {
                this.TokenIndex = tokenIndex;
                this.AbsoluteCharIndex = absoluteCharIndex;
                this.VisiblePosition = visiblePosition;
                this.LineCharIndex = lineCharIndex;
            }
        }

        #endregion

        #region Fields

        private readonly List<Token> _tokens;
        private readonly List<TokenPosition> _tokenPositions;

        #endregion

        #region Constructor

        public TokenCollection()
        {
            _tokens = new List<Token>();
            _tokenPositions = new List<TokenPosition>();
        }

        #endregion

        #region Public

        public void Add(Token token, TokenPosition tokenPosition)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            _tokens.Add(token);
            _tokenPositions.Add(tokenPosition);
        }

        public TokenPosition GetPosition(int index)
        {
            return _tokenPositions[index];
        }

        #endregion

        #region IReadOnlyList<Token>

        public IEnumerator<Token> GetEnumerator() => _tokens.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public int Count => _tokens.Count;

        public Token this[int index] => _tokens[index];

        #endregion
    }
}
