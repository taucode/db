using System;

namespace TauCode.Db.Utils.Parsing.Core
{
    public abstract class Token : IEquatable<Token>
    {
        protected abstract int GetHashCodeImpl();

        public abstract bool Equals(Token other);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Token)obj);
        }

        public override int GetHashCode()
        {
            return this.GetHashCodeImpl();
        }

        public static bool operator ==(Token a, Token b)
        {
            return !(a is null) && a.Equals(b);
        }

        public static bool operator !=(Token a, Token b)
        {
            return !(a == b);
        }
    }
}
