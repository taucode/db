using System;
using System.Data.Common;

namespace TauCode.Db.Exceptions
{
    // todo: get rid of these exceptions. TauCodeDbException would be enough.
    [Serializable]
    public class ScriptBuildingException : DbException
    {
        public ScriptBuildingException()
        {
        }

        public ScriptBuildingException(string message)
            : base(message)
        {
        }

        public ScriptBuildingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
