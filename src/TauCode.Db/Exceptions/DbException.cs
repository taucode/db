using System;

namespace TauCode.Db.Exceptions
{
    [Serializable]
    public class DbException : System.Data.Common.DbException
    {
        public DbException()
        {
        }

        public DbException(string message)
            : base(message)
        {
        }

        public DbException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
