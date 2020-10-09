using System;

namespace TauCode.Db.Exceptions
{
    [Serializable]
    public class TauDbException : System.Data.Common.DbException
    {
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
