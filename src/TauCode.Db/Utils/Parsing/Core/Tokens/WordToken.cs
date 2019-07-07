using System;
using System.Diagnostics;

namespace TauCode.Db.Utils.Parsing.Core.Tokens
{
    [DebuggerDisplay("{" + nameof(Word) + "}")]
    public class WordToken : Token
    {
        public WordToken(string word)
        {
            this.Word = word ?? throw new ArgumentNullException(nameof(word));
        }

        public string Word { get; }

        protected override int GetHashCodeImpl()
        {
            return this.Word.GetHashCode();
        }

        public override bool Equals(Token other)
        {
            if (!(other is WordToken otherWordToken))
            {
                return false;
            }

            return this.Word == otherWordToken.Word;
        }

        public override string ToString()
        {
            return this.Word;
        }
    }
}
