using System;

namespace TauCode.Db.Utils.Parsing.Core
{
    public class ParsingFlow
    {
        private readonly Token[] _tokens;
        
        private int _position;

        public ParsingFlow(Token[] tokens)
        {
            Context = new ParsingContext();
            _tokens = tokens ?? throw new ArgumentNullException(nameof(tokens));
            _position = 0;
        }

        public bool IsEnd => _position == _tokens.Length;

        public ParsingContext Context { get; }

        public void Advance(int forwardShift)
        {
            if (forwardShift < 0)
            {
                throw new ArgumentOutOfRangeException();
            }

            if (_position + forwardShift > _tokens.Length)
            {
                throw new ArgumentException($"Forward shift too large.", nameof(forwardShift));
            }

            _position += forwardShift;
        }

        public Token CurrentToken => this.GetToken(0);

        public Token GetToken(int forwardShift)
        {
            var index = _position + forwardShift;
            if (index >= _tokens.Length)
            {
                return null;
            }

            return _tokens[index];
        }
    }
}
