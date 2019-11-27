using System;
using System.Data.Common;

namespace TauCode.Db.Exceptions
{
    [Serializable]
    public class ObjectNotFoundException : DbException
    {
        public ObjectNotFoundException()
            : base("Object not found.")
        {
        }

        public ObjectNotFoundException(string message)
            : base(message)
        {
        }

        public ObjectNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ObjectNotFoundException(string message, string objectName)
            : base(message)
        {
            this.ObjectName = objectName;
        }

        public string ObjectName { get; }
    }
}
