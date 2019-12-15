using System;
using System.Data.Common;

namespace TauCode.Db.Exceptions
{
    [Serializable]
    public class TauCodeDbException : DbException
    {
        public TauCodeDbException()
        {
        }

        public TauCodeDbException(string message)
            : base(message)
        {
        }

        public TauCodeDbException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
