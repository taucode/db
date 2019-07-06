using System;

namespace TauCode.Db.Exceptions
{
    [Serializable]
    public class ScriptBuildingException : Exception
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
