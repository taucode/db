using System;
using System.Diagnostics;

namespace TauCode.Db.Utils.Parsing.Core.Tokens
{
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    public class StringToken : Token
    {
        public StringToken(string value)
        {
            this.Value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public string Value { get; }

        protected override int GetHashCodeImpl()
        {
            return this.Value.GetHashCode();
        }

        public override bool Equals(Token other)
        {
            if (other is StringToken otherStringToken)
            {
                return this.Value == otherStringToken.Value;
            }

            return false;
        }
    }
}
