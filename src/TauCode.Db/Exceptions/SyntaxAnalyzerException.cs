using System;

namespace TauCode.Db.Exceptions
{
    [Serializable]
    public class SyntaxAnalyzerException : Exception
    {
        public SyntaxAnalyzerException()
        {
        }

        public SyntaxAnalyzerException(string message)
            : base(message)
        {
        }

        public SyntaxAnalyzerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
