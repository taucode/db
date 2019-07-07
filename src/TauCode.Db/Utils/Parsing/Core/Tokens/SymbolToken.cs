using System.Diagnostics;

namespace TauCode.Db.Utils.Parsing.Core.Tokens
{
    [DebuggerDisplay("{" + nameof(ToString) + "()}")]
    public class SymbolToken : Token
    {
        public static readonly SymbolToken OpeningRoundBracketSymbol = new SymbolToken(SymbolName.OpeningRoundBracket);
        public static readonly SymbolToken ClosingRoundBracketSymbol = new SymbolToken(SymbolName.ClosingRoundBracket);
        public static readonly SymbolToken CommaSymbol = new SymbolToken(SymbolName.Comma);
        public static readonly SymbolToken SemicolonSymbol = new SymbolToken(SymbolName.Semicolon);

        private SymbolToken(SymbolName name)
        {
            this.Name = name;
        }

        public SymbolName Name { get; }
        

        protected override int GetHashCodeImpl()
        {
            return this.Name.GetHashCode();
        }

        public override bool Equals(Token other)
        {
            if (!(other is SymbolToken otherSymbolToken))
            {
                return false;
            }

            return this.Name == otherSymbolToken.Name;
        }

        public override string ToString()
        {
            return this.Name.GetSymbolNameCaption();
        }
    }
}
