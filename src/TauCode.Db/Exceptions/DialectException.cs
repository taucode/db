using System;

namespace TauCode.Db.Exceptions
{
    [Serializable]
    public class DialectException : Exception
    {
        public DialectException()
        {
        }

        public DialectException(string message)
            : base(message)
        {

        }

        public DialectException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
