using System;

namespace TauCode.Db.Utils.Parsing.Core.Tokens
{
    public class IdentifierToken : Token
    {
        public IdentifierToken(string identifier)
        {
            this.Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        }

        public string Identifier { get; }

        protected override int GetHashCodeImpl()
        {
            return this.Identifier.GetHashCode();
        }

        public override bool Equals(Token other)
        {
            if (!(other is IdentifierToken otherIdentifierToken))
            {
                return false;
            }

            return this.Identifier == otherIdentifierToken.Identifier;
        }

        public override string ToString()
        {
            return this.Identifier;
        }
    }
}
