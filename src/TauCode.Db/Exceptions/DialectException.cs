using System;
using System.Data.Common;

namespace TauCode.Db.Exceptions
{
    [Serializable]
    public class DialectException : DbException
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
