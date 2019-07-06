using System;

namespace TauCode.Db.Exceptions
{
    [Serializable]
    public class TokenizerException : Exception
    {
        public TokenizerException()
        {
        }

        public TokenizerException(string message)
            : base(message)
        {
        }

        public TokenizerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
