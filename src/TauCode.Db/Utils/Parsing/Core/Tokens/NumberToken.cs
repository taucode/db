using System;
using System.Diagnostics;

namespace TauCode.Db.Utils.Parsing.Core.Tokens
{
    [DebuggerDisplay("{" + nameof(Number) + "}")]
    public class NumberToken : Token
    {
        public NumberToken(string number)
        {
            this.Number = number ?? throw new ArgumentNullException(nameof(number));
        }

        public string Number { get; }

        protected override int GetHashCodeImpl()
        {
            return this.Number.GetHashCode();
        }

        public override bool Equals(Token other)
        {
            if (!(other is NumberToken otherNumberToken))
            {
                return false;
            }

            return this.Number == otherNumberToken.Number;
        }

        public override string ToString()
        {
            return this.Number;
        }
    }
}
