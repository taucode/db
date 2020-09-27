using System;

namespace TauCode.Db.Exceptions
{
    // todo: why on earth gonna need this?!
    [Serializable]
    public class TauDbException : System.Data.Common.DbException
    {
        public TauDbException()
        {
        }

        public TauDbException(string message)
            : base(message)
        {
        }

        public TauDbException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
